namespace MotoZavodyWeb.Models
{
    public class Kolobezka
    {
        public int IdKolobezka { get; set; }
        public string Model { get; set; } = null!;
        public string Znacka { get; set; } = null!;

        public int IdTypKolobezky { get; set; }
        public TypKolobezky TypKolobezky { get; set; } = null!;

        public ICollection<JezdiNa> JezdiNa { get; set; } = new List<JezdiNa>();
    }
}