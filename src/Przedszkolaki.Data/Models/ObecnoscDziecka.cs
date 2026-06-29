namespace Przedszkolaki.Data.Models;

public class ObecnoscDziecka
{
    public int Id { get; set; }
    public int DzieckoId { get; set; }
    public DateTime Data { get; set; }
    public bool CzyObecne { get; set; }
    public TimeSpan? GodzinaPrzyjscia { get; set; }
    public TimeSpan? GodzinaOdejscia { get; set; }

    public Dziecko Dziecko { get; set; } = null!;
}
