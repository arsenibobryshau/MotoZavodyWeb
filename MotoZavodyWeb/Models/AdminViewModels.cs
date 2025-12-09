namespace MotoZavodyWeb.Models
{
    // Model pro řádek v systémovém katalogu (Bod 30)
    public class SystemovyObjekt
    {
        public string Nazev { get; set; } = string.Empty;
        public string Typ { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}