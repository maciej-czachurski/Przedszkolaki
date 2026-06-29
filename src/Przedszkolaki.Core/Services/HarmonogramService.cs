using Microsoft.EntityFrameworkCore;
using Przedszkolaki.Data;
using Przedszkolaki.Data.Enums;
using Przedszkolaki.Data.Models;

namespace Przedszkolaki.Core.Services;

public class HarmonogramService
{
    private readonly PrzedszkolakiDbContext _db;

    public HarmonogramService(PrzedszkolakiDbContext db) => _db = db;

    public async Task<List<HarmonogramWpis>> GetByMiesiacAsync(int rok, int miesiac)
    {
        var start = new DateTime(rok, miesiac, 1);
        var end = start.AddMonths(1);
        return await _db.HarmonogramWpisy
            .Include(h => h.Pracownik)
            .Include(h => h.Grupa)
            .Where(h => h.Data >= start && h.Data < end)
            .OrderBy(h => h.Data).ThenBy(h => h.Pracownik.Nazwisko)
            .ToListAsync();
    }

    public async Task<List<HarmonogramWpis>> GetByPracownikAndMiesiacAsync(int pracownikId, int rok, int miesiac)
    {
        var start = new DateTime(rok, miesiac, 1);
        var end = start.AddMonths(1);
        return await _db.HarmonogramWpisy
            .Where(h => h.PracownikId == pracownikId && h.Data >= start && h.Data < end)
            .OrderBy(h => h.Data)
            .ToListAsync();
    }

    public async Task AddAsync(HarmonogramWpis wpis)
    {
        _db.HarmonogramWpisy.Add(wpis);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(HarmonogramWpis wpis)
    {
        _db.HarmonogramWpisy.Update(wpis);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var h = await _db.HarmonogramWpisy.FindAsync(id);
        if (h != null)
        {
            _db.HarmonogramWpisy.Remove(h);
            await _db.SaveChangesAsync();
        }
    }

    public async Task UsunMiesiacAsync(int rok, int miesiac)
    {
        var start = new DateTime(rok, miesiac, 1);
        var end = start.AddMonths(1);
        var wpisy = await _db.HarmonogramWpisy
            .Where(h => h.Data >= start && h.Data < end)
            .ToListAsync();
        _db.HarmonogramWpisy.RemoveRange(wpisy);
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Generuje miesięczny harmonogram zmian.
    /// Algorytm zachłanny: dla każdego dnia roboczego w miesiącu, dla każdej grupy
    /// dobiera pracowników spełniających wymagania obsady, pomijając osoby na urlopie.
    /// Rozkłada zmiany równomiernie według dotychczasowej liczby przydzielonych godzin.
    /// </summary>
    public async Task<List<HarmonogramWpis>> GenerujHarmonogramAsync(int rok, int miesiac)
    {
        var start = new DateTime(rok, miesiac, 1);
        var end = start.AddMonths(1).AddDays(-1);

        var grupy = await _db.Grupy
            .Include(g => g.PracownicyGrupy).ThenInclude(pg => pg.Pracownik)
            .Include(g => g.WymaganiaObsady)
            .ToListAsync();

        var urlopyZatwierdzone = await _db.Urlopy
            .Where(u => u.Status == StatusUrlopu.Zatwierdzony
                        && u.DataOd <= end && u.DataDo >= start)
            .ToListAsync();

        var wynik = new List<HarmonogramWpis>();
        var godzinyPracownika = new Dictionary<int, double>();

        for (var data = start; data <= end; data = data.AddDays(1))
        {
            if (data.DayOfWeek == DayOfWeek.Saturday || data.DayOfWeek == DayOfWeek.Sunday)
                continue;

            foreach (var grupa in grupy)
            {
                var pracownicyGrupy = grupa.PracownicyGrupy
                    .Where(pg => pg.DataDo == null)
                    .ToList();

                var naUrlopie = urlopyZatwierdzone
                    .Where(u => u.DataOd.Date <= data.Date && u.DataDo.Date >= data.Date)
                    .Select(u => u.PracownikId)
                    .ToHashSet();

                var dostepni = pracownicyGrupy
                    .Where(pg => !naUrlopie.Contains(pg.PracownikId) && pg.Pracownik.CzyAktywny)
                    .ToList();

                foreach (var wymag in grupa.WymaganiaObsady)
                {
                    var kandydaci = dostepni
                        .Where(pg => pg.Rola == wymag.TypPracownika)
                        .OrderBy(pg => godzinyPracownika.GetValueOrDefault(pg.PracownikId, 0))
                        .Take(wymag.MinimalnaLiczba)
                        .ToList();

                    foreach (var pg in kandydaci)
                    {
                        var dlugosc = (wymag.GodzinaDo - wymag.GodzinaOd).TotalHours;
                        godzinyPracownika[pg.PracownikId] =
                            godzinyPracownika.GetValueOrDefault(pg.PracownikId, 0) + dlugosc;

                        wynik.Add(new HarmonogramWpis
                        {
                            PracownikId = pg.PracownikId,
                            GrupaId = grupa.Id,
                            Data = data,
                            GodzinaOd = wymag.GodzinaOd,
                            GodzinaDo = wymag.GodzinaDo
                        });
                    }
                }
            }
        }

        return wynik;
    }

    public async Task ZapiszWygenerowanyHarmonogramAsync(List<HarmonogramWpis> wpisy, int rok, int miesiac)
    {
        await UsunMiesiacAsync(rok, miesiac);
        _db.HarmonogramWpisy.AddRange(wpisy);
        await _db.SaveChangesAsync();
    }
}
