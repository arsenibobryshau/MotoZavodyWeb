namespace MotoZavodyWeb.Models
{
    public class ZavodFilterModel
    {
        public int? Rok { get; set; }
        public int? TypZavoduId { get; set; }
        public int? HodnoceniId { get; set; }
        public string? Cena { get; set; }  // asc / desc
    }
}
