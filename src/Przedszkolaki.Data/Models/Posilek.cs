using Przedszkolaki.Data.Enums;

namespace Przedszkolaki.Data.Models;

public class Posilek
{
    public int Id { get; set; }
    public int DzieckoId { get; set; }
    public DateTime Data { get; set; }
    public TypPosilku TypPosilku { get; set; }
    public bool CzyZamowiony { get; set; }
    public bool CzyWydany { get; set; }

    public Dziecko Dziecko { get; set; } = null!;
}
