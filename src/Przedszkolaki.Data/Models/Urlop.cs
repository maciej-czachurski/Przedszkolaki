using Przedszkolaki.Data.Enums;

namespace Przedszkolaki.Data.Models;

public class Urlop
{
    public int Id { get; set; }
    public int PracownikId { get; set; }
    public DateTime DataOd { get; set; }
    public DateTime DataDo { get; set; }
    public TypUrlopu Typ { get; set; }
    public StatusUrlopu Status { get; set; } = StatusUrlopu.Zlozony;
    public string Uwagi { get; set; } = string.Empty;

    public Pracownik Pracownik { get; set; } = null!;
}
