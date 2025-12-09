using System.ComponentModel.DataAnnotations.Schema;

namespace MotoZavodyWeb.Models
{
    public class Ucast
    {
        [Column("ID_ZAVODNIK")]
        public int IdZavodnik { get; set; }
        [Column("ID_ZAVOD")]
        public int IdZavod { get; set; }
        [Column("PORADI")]
        public string? Poradi { get; set; }    // napø. "1.", "DNF"
        [Column("VYSLEDEK")]
        public string? Vysledek { get; set; }  // èas nebo popis
        [Column("ID_PLATBY")]
        public int? IdPlatby { get; set; }

        public Zavodnik Zavodnik { get; set; } = null!;
        public Zavod Zavod { get; set; } = null!;
        public Platba? Platba { get; set; }
    }
}
