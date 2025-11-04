namespace MotoZavodyWeb.Models
{
    public class TypKolobezky
    {
        public int IdTypKolobezky { get; set; }
        public string Nazev { get; set; } = null!;

        public ICollection<Kolobezka> Kolobezky { get; set; } = new List<Kolobezka>();
    }
}