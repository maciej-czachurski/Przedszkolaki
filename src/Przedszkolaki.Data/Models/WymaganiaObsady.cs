using Przedszkolaki.Data.Enums;

namespace Przedszkolaki.Data.Models;

public class WymaganiaObsady
{
    public int Id { get; set; }
    public int GrupaId { get; set; }
    public TypPracownika TypPracownika { get; set; }
    public int MinimalnaLiczba { get; set; }
    public TimeSpan GodzinaOd { get; set; }
    public TimeSpan GodzinaDo { get; set; }

    public Grupa Grupa { get; set; } = null!;
}
