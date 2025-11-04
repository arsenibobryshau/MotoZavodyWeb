namespace MotoZavodyWeb.Models
{
    public class TypZavodu
    {
        public int IdTypZavodu { get; set; }
        public string Nazev { get; set; } = null!;

        public ICollection<Zavod> Zavody { get; set; } = new List<Zavod>();
    }
}