namespace MotoZavodyWeb.Models
{
    public class JezdiNa
    {
        public int IdZavodnik { get; set; }
        public int IdKolobezka { get; set; }

        public Zavodnik Zavodnik { get; set; } = null!;
        public Kolobezka Kolobezka { get; set; } = null!;
    }
}