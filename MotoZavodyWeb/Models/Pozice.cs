namespace MotoZavodyWeb.Models
{
    public class Pozice
    {
        public int IdPozice { get; set; }
        public string Nazev { get; set; } = null!;

        public ICollection<Zamestnanec> Zamestnanci { get; set; } = new List<Zamestnanec>();
    }
}