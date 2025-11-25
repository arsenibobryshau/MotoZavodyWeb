namespace MotoZavodyWeb.Models
{
    public class ZavodPrehledItem
    {
        public int IdZavod { get; set; }
        public string Nazev { get; set; } = "";
        public int Rok { get; set; }
        public string TypZavodu { get; set; } = "";
        public int TypZavoduId { get; set; }
        public string Hodnoceni { get; set; } = "";
        public int HodnoceniId { get; set; }
        public decimal Startovne { get; set; }
    }
}
