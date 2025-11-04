namespace MotoZavodyWeb.Models
{
    public class Ucast
    {
        public int IdZavodnik { get; set; }
        public int IdZavod { get; set; }
        public string? Poradi { get; set; }    // napø. "1.", "DNF"
        public string? Vysledek { get; set; }  // èas nebo popis
        public int? IdPlatby { get; set; }

        public Zavodnik Zavodnik { get; set; } = null!;
        public Zavod Zavod { get; set; } = null!;
        public Platba? Platba { get; set; }
    }
}
