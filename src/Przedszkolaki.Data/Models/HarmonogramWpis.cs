namespace Przedszkolaki.Data.Models;

public class HarmonogramWpis
{
    public int Id { get; set; }
    public int PracownikId { get; set; }
    public int? GrupaId { get; set; }
    public DateTime Data { get; set; }
    public TimeSpan GodzinaOd { get; set; }
    public TimeSpan GodzinaDo { get; set; }
    public string Uwagi { get; set; } = string.Empty;

    public Pracownik Pracownik { get; set; } = null!;
    public Grupa? Grupa { get; set; }
}
