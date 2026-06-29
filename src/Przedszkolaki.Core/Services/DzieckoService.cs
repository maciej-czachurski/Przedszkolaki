using Microsoft.EntityFrameworkCore;
using Przedszkolaki.Data;
using Przedszkolaki.Data.Models;

namespace Przedszkolaki.Core.Services;

public class DzieckoService
{
    private readonly PrzedszkolakiDbContext _db;

    public DzieckoService(PrzedszkolakiDbContext db) => _db = db;

    public async Task<List<Dziecko>> GetAllAsync()
        => await _db.Dzieci.Include(d => d.Grupa).OrderBy(d => d.Nazwisko).ThenBy(d => d.Imie).ToListAsync();

    public async Task<List<Dziecko>> GetByGrupaAsync(int grupaId)
        => await _db.Dzieci.Where(d => d.GrupaId == grupaId).OrderBy(d => d.Nazwisko).ToListAsync();

    public async Task<Dziecko?> GetByIdAsync(int id)
        => await _db.Dzieci.Include(d => d.Grupa).FirstOrDefaultAsync(d => d.Id == id);

    public async Task AddAsync(Dziecko dziecko)
    {
        _db.Dzieci.Add(dziecko);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Dziecko dziecko)
    {
        _db.Dzieci.Update(dziecko);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var d = await _db.Dzieci.FindAsync(id);
        if (d != null)
        {
            _db.Dzieci.Remove(d);
            await _db.SaveChangesAsync();
        }
    }
}
