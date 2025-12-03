namespace MotoZavodyWeb.Models
{
    public class ZavodHierarchyView
    {
        public string TypUzlu { get; set; } = string.Empty; // ROK / ZAVOD
        public string Nazev { get; set; } = string.Empty;   // rok nebo název závodu
        public int Rok { get; set; }
        public int? IdZavod { get; set; }
        public int Uroven { get; set; }
    }
}
