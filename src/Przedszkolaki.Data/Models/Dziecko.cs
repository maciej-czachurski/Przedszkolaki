using Przedszkolaki.Data.Enums;

namespace Przedszkolaki.Data.Models;

public class Dziecko
{
    public int Id { get; set; }
    public string Imie { get; set; } = string.Empty;
    public string Nazwisko { get; set; } = string.Empty;
    public DateTime DataUrodzenia { get; set; }
    public int? GrupaId { get; set; }
    public StatusDziecka Status { get; set; } = StatusDziecka.Aktywny;
    public string ImieNazwiskoOpiekuna { get; set; } = string.Empty;
    public string TelefonOpiekuna { get; set; } = string.Empty;
    public string EmailOpiekuna { get; set; } = string.Empty;

    public Grupa? Grupa { get; set; }
    public ICollection<ObecnoscDziecka> Obecnosci { get; set; } = new List<ObecnoscDziecka>();
    public ICollection<Posilek> Posilki { get; set; } = new List<Posilek>();

    public string PelneNazwisko => $"{Nazwisko} {Imie}";
}
