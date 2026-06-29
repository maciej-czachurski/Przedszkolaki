using Microsoft.EntityFrameworkCore;
using Przedszkolaki.Data;
using Przedszkolaki.Data.Enums;
using Przedszkolaki.Data.Models;

namespace Przedszkolaki.Core.Services;

public class PosilekService
{
    private readonly PrzedszkolakiDbContext _db;

    public PosilekService(PrzedszkolakiDbContext db) => _db = db;

    public async Task<List<Posilek>> GetByDateAsync(DateTime data)
        => await _db.Posilki
            .Include(p => p.Dziecko)
            .Where(p => p.Data.Date == data.Date)
            .OrderBy(p => p.Dziecko.Nazwisko)
            .ToListAsync();

    public async Task<List<Posilek>> GetByDzieckoAndDateAsync(int dzieckoId, DateTime data)
        => await _db.Posilki
            .Where(p => p.DzieckoId == dzieckoId && p.Data.Date == data.Date)
            .ToListAsync();

    public async Task ZapiszPosilkiAsync(List<Posilek> posilki)
    {
        foreach (var p in posilki)
        {
            var existing = await _db.Posilki
                .FirstOrDefaultAsync(x => x.DzieckoId == p.DzieckoId && x.Data.Date == p.Data.Date && x.TypPosilku == p.TypPosilku);
            if (existing == null)
                _db.Posilki.Add(p);
            else
            {
                existing.CzyZamowiony = p.CzyZamowiony;
                existing.CzyWydany = p.CzyWydany;
            }
        }
        await _db.SaveChangesAsync();
    }

    public async Task<Dictionary<TypPosilku, int>> GetRaportDziennyAsync(DateTime data)
    {
        var result = await _db.Posilki
            .Where(p => p.Data.Date == data.Date && p.CzyZamowiony)
            .GroupBy(p => p.TypPosilku)
            .Select(g => new { Typ = g.Key, Liczba = g.Count() })
            .ToListAsync();
        return result.ToDictionary(x => x.Typ, x => x.Liczba);
    }

    public async Task<List<(DateTime Data, TypPosilku Typ, int Liczba)>> GetRaportMiesięcznyAsync(int rok, int miesiac)
    {
        var start = new DateTime(rok, miesiac, 1);
        var end = start.AddMonths(1);
        var result = await _db.Posilki
            .Where(p => p.Data >= start && p.Data < end && p.CzyZamowiony)
            .GroupBy(p => new { p.Data, p.TypPosilku })
            .Select(g => new { g.Key.Data, g.Key.TypPosilku, Liczba = g.Count() })
            .OrderBy(x => x.Data).ThenBy(x => x.TypPosilku)
            .ToListAsync();
        return result.Select(x => (x.Data, x.TypPosilku, x.Liczba)).ToList();
    }
}
