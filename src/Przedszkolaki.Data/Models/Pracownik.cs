using Przedszkolaki.Data.Enums;

namespace Przedszkolaki.Data.Models;

public class Pracownik
{
    public int Id { get; set; }
    public string Imie { get; set; } = string.Empty;
    public string Nazwisko { get; set; } = string.Empty;
    public string Pesel { get; set; } = string.Empty;
    public TypPracownika TypPracownika { get; set; }
    public decimal Etat { get; set; } = 1.0m;
    public DateTime DataZatrudnienia { get; set; }
    public bool CzyAktywny { get; set; } = true;

    public ICollection<PracownikGrupa> PracownicyGrupy { get; set; } = new List<PracownikGrupa>();
    public ICollection<Urlop> Urlopy { get; set; } = new List<Urlop>();
    public ICollection<HarmonogramWpis> HarmonogramWpisy { get; set; } = new List<HarmonogramWpis>();

    public string PelneNazwisko => $"{Nazwisko} {Imie}";
}
