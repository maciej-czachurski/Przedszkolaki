using Microsoft.EntityFrameworkCore;
using Przedszkolaki.Data;
using Przedszkolaki.Data.Enums;
using Przedszkolaki.Data.Models;

namespace Przedszkolaki.Core.Services;

public class PracownikService
{
    private readonly PrzedszkolakiDbContext _db;

    public PracownikService(PrzedszkolakiDbContext db) => _db = db;

    public async Task<List<Pracownik>> GetAllAsync(bool tylkoAktywni = false)
    {
        var query = _db.Pracownicy.Include(p => p.PracownicyGrupy).ThenInclude(pg => pg.Grupa).AsQueryable();
        if (tylkoAktywni)
            query = query.Where(p => p.CzyAktywny);
        return await query.OrderBy(p => p.Nazwisko).ThenBy(p => p.Imie).ToListAsync();
    }

    public async Task<Pracownik?> GetByIdAsync(int id)
        => await _db.Pracownicy.Include(p => p.PracownicyGrupy).ThenInclude(pg => pg.Grupa)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task AddAsync(Pracownik pracownik)
    {
        _db.Pracownicy.Add(pracownik);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Pracownik pracownik)
    {
        _db.Pracownicy.Update(pracownik);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var p = await _db.Pracownicy.FindAsync(id);
        if (p != null)
        {
            p.CzyAktywny = false;
            await _db.SaveChangesAsync();
        }
    }

    public async Task PrzypiszDoGrupyAsync(int pracownikId, int grupaId, TypPracownika rola)
    {
        var istniejacy = await _db.PracownicyGrupy
            .FirstOrDefaultAsync(pg => pg.PracownikId == pracownikId && pg.GrupaId == grupaId && pg.DataDo == null);
        if (istniejacy == null)
        {
            _db.PracownicyGrupy.Add(new PracownikGrupa
            {
                PracownikId = pracownikId,
                GrupaId = grupaId,
                Rola = rola,
                DataOd = DateTime.Today
            });
            await _db.SaveChangesAsync();
        }
    }

    public async Task UsunZGrupyAsync(int pracownikGrupaId)
    {
        var pg = await _db.PracownicyGrupy.FindAsync(pracownikGrupaId);
        if (pg != null)
        {
            pg.DataDo = DateTime.Today;
            await _db.SaveChangesAsync();
        }
    }
}
