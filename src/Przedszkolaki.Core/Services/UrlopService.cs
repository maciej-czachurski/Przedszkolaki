using Microsoft.EntityFrameworkCore;
using Przedszkolaki.Data;
using Przedszkolaki.Data.Enums;
using Przedszkolaki.Data.Models;

namespace Przedszkolaki.Core.Services;

public class UrlopService
{
    private readonly PrzedszkolakiDbContext _db;

    public UrlopService(PrzedszkolakiDbContext db) => _db = db;

    public async Task<List<Urlop>> GetAllAsync()
        => await _db.Urlopy.Include(u => u.Pracownik).OrderByDescending(u => u.DataOd).ToListAsync();

    public async Task<List<Urlop>> GetByPracownikAsync(int pracownikId)
        => await _db.Urlopy.Where(u => u.PracownikId == pracownikId).OrderByDescending(u => u.DataOd).ToListAsync();

    public async Task<List<Urlop>> GetByMiesiacAsync(int rok, int miesiac)
    {
        var start = new DateTime(rok, miesiac, 1);
        var end = start.AddMonths(1).AddDays(-1);
        return await _db.Urlopy
            .Include(u => u.Pracownik)
            .Where(u => u.DataOd <= end && u.DataDo >= start)
            .OrderBy(u => u.DataOd)
            .ToListAsync();
    }

    public async Task<List<Urlop>> GetZatwierdzonePrzezOkresAsync(DateTime start, DateTime end)
        => await _db.Urlopy
            .Include(u => u.Pracownik)
            .Where(u => u.Status == StatusUrlopu.Zatwierdzony && u.DataOd <= end && u.DataDo >= start)
            .ToListAsync();

    public async Task AddAsync(Urlop urlop)
    {
        _db.Urlopy.Add(urlop);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(int urlopId, StatusUrlopu nowyStatus)
    {
        var u = await _db.Urlopy.FindAsync(urlopId);
        if (u != null)
        {
            u.Status = nowyStatus;
            await _db.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        var u = await _db.Urlopy.FindAsync(id);
        if (u != null)
        {
            _db.Urlopy.Remove(u);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<bool> SprawdzKonfliktObsadyAsync(DateTime data, int grupaId)
    {
        var pracownicyGrupy = await _db.PracownicyGrupy
            .Where(pg => pg.GrupaId == grupaId && pg.DataDo == null)
            .Select(pg => pg.PracownikId)
            .ToListAsync();

        var naUrlopie = await _db.Urlopy
            .Where(u => pracownicyGrupy.Contains(u.PracownikId)
                        && u.Status == StatusUrlopu.Zatwierdzony
                        && u.DataOd <= data && u.DataDo >= data)
            .CountAsync();

        return naUrlopie >= pracownicyGrupy.Count;
    }
}
