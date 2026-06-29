using Przedszkolaki.Data.Enums;

namespace Przedszkolaki.Data.Models;

public class PracownikGrupa
{
    public int Id { get; set; }
    public int PracownikId { get; set; }
    public int GrupaId { get; set; }
    public TypPracownika Rola { get; set; }
    public DateTime DataOd { get; set; }
    public DateTime? DataDo { get; set; }

    public Pracownik Pracownik { get; set; } = null!;
    public Grupa Grupa { get; set; } = null!;
}
