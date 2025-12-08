using System.ComponentModel.DataAnnotations.Schema;

namespace MotoZavodyWeb.Models
{
    public class Uzivatel
    {
        public int IdUzivatel { get; set; }

        public string Jmeno { get; set; } = string.Empty;
        [Column("PRIJMENI")]
        public string? Prijmeni { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Heslo { get; set; } = string.Empty;

        // USER / ADMIN
        public string Role { get; set; } = "USER";

        public DateTime DatumVytvoreni { get; set; }

        public DateTime? DatumPoslednihoPrihlaseni { get; set; }

        // Vazba na závodníka (může být NULL)
        public int? IdZavodnik { get; set; }

        public Zavodnik? Zavodnik { get; set; }
        
        [Column("PROFIL_FOTO")]
        public byte[]? ProfilFoto { get; set; }
    }
}
