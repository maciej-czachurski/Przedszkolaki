namespace Przedszkolaki.Data.Models;

public class Grupa
{
    public int Id { get; set; }
    public string Nazwa { get; set; } = string.Empty;
    public string Opis { get; set; } = string.Empty;
    public TimeSpan GodzinaOd { get; set; }
    public TimeSpan GodzinaDo { get; set; }
    public int WiekMinLat { get; set; }
    public int WiekMaxLat { get; set; }

    public ICollection<PracownikGrupa> PracownicyGrupy { get; set; } = new List<PracownikGrupa>();
    public ICollection<Dziecko> Dzieci { get; set; } = new List<Dziecko>();
    public ICollection<HarmonogramWpis> HarmonogramWpisy { get; set; } = new List<HarmonogramWpis>();
    public ICollection<WymaganiaObsady> WymaganiaObsady { get; set; } = new List<WymaganiaObsady>();
}
