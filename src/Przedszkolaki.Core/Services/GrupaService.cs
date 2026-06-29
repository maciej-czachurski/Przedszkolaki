using Microsoft.EntityFrameworkCore;
using Przedszkolaki.Data;
using Przedszkolaki.Data.Models;

namespace Przedszkolaki.Core.Services;

public class GrupaService
{
    private readonly PrzedszkolakiDbContext _db;

    public GrupaService(PrzedszkolakiDbContext db) => _db = db;

    public async Task<List<Grupa>> GetAllAsync()
        => await _db.Grupy
            .Include(g => g.PracownicyGrupy).ThenInclude(pg => pg.Pracownik)
            .Include(g => g.WymaganiaObsady)
            .OrderBy(g => g.Nazwa)
            .ToListAsync();

    public async Task<Grupa?> GetByIdAsync(int id)
        => await _db.Grupy
            .Include(g => g.PracownicyGrupy).ThenInclude(pg => pg.Pracownik)
            .Include(g => g.WymaganiaObsady)
            .FirstOrDefaultAsync(g => g.Id == id);

    public async Task AddAsync(Grupa grupa)
    {
        _db.Grupy.Add(grupa);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Grupa grupa)
    {
        _db.Grupy.Update(grupa);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var g = await _db.Grupy.FindAsync(id);
        if (g != null)
        {
            _db.Grupy.Remove(g);
            await _db.SaveChangesAsync();
        }
    }
}
