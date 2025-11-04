namespace MotoZavodyWeb.Models
{
    public class Hodnoceni
    {
        public int IdHodnoceni { get; set; }
        public string Metoda { get; set; } = null!;

        public ICollection<Zavod> Zavody { get; set; } = new List<Zavod>();
    }
}