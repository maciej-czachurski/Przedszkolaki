using Microsoft.EntityFrameworkCore;
using Przedszkolaki.Data.Models;

namespace Przedszkolaki.Data;

public class PrzedszkolakiDbContext : DbContext
{
    public DbSet<Pracownik> Pracownicy { get; set; }
    public DbSet<Grupa> Grupy { get; set; }
    public DbSet<PracownikGrupa> PracownicyGrupy { get; set; }
    public DbSet<Dziecko> Dzieci { get; set; }
    public DbSet<Urlop> Urlopy { get; set; }
    public DbSet<HarmonogramWpis> HarmonogramWpisy { get; set; }
    public DbSet<ObecnoscDziecka> ObecnosciDzieci { get; set; }
    public DbSet<Posilek> Posilki { get; set; }
    public DbSet<WymaganiaObsady> WymaganiaObsady { get; set; }

    public PrzedszkolakiDbContext(DbContextOptions<PrzedszkolakiDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Pracownik>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Imie).IsRequired().HasMaxLength(100);
            e.Property(p => p.Nazwisko).IsRequired().HasMaxLength(100);
            e.Property(p => p.Pesel).HasMaxLength(11);
            e.Property(p => p.Etat).HasPrecision(4, 2);
        });

        modelBuilder.Entity<Grupa>(e =>
        {
            e.HasKey(g => g.Id);
            e.Property(g => g.Nazwa).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<PracownikGrupa>(e =>
        {
            e.HasKey(pg => pg.Id);
            e.HasOne(pg => pg.Pracownik)
                .WithMany(p => p.PracownicyGrupy)
                .HasForeignKey(pg => pg.PracownikId);
            e.HasOne(pg => pg.Grupa)
                .WithMany(g => g.PracownicyGrupy)
                .HasForeignKey(pg => pg.GrupaId);
        });

        modelBuilder.Entity<Dziecko>(e =>
        {
            e.HasKey(d => d.Id);
            e.Property(d => d.Imie).IsRequired().HasMaxLength(100);
            e.Property(d => d.Nazwisko).IsRequired().HasMaxLength(100);
            e.HasOne(d => d.Grupa)
                .WithMany(g => g.Dzieci)
                .HasForeignKey(d => d.GrupaId)
                .IsRequired(false);
        });

        modelBuilder.Entity<Urlop>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasOne(u => u.Pracownik)
                .WithMany(p => p.Urlopy)
                .HasForeignKey(u => u.PracownikId);
        });

        modelBuilder.Entity<HarmonogramWpis>(e =>
        {
            e.HasKey(h => h.Id);
            e.HasOne(h => h.Pracownik)
                .WithMany(p => p.HarmonogramWpisy)
                .HasForeignKey(h => h.PracownikId);
            e.HasOne(h => h.Grupa)
                .WithMany(g => g.HarmonogramWpisy)
                .HasForeignKey(h => h.GrupaId)
                .IsRequired(false);
        });

        modelBuilder.Entity<ObecnoscDziecka>(e =>
        {
            e.HasKey(o => o.Id);
            e.HasOne(o => o.Dziecko)
                .WithMany(d => d.Obecnosci)
                .HasForeignKey(o => o.DzieckoId);
        });

        modelBuilder.Entity<Posilek>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasOne(p => p.Dziecko)
                .WithMany(d => d.Posilki)
                .HasForeignKey(p => p.DzieckoId);
        });

        modelBuilder.Entity<WymaganiaObsady>(e =>
        {
            e.HasKey(w => w.Id);
            e.HasOne(w => w.Grupa)
                .WithMany(g => g.WymaganiaObsady)
                .HasForeignKey(w => w.GrupaId);
        });
    }
}
