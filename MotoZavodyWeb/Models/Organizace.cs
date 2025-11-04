namespace MotoZavodyWeb.Models
{
    public class Organizace
    {
        public int IdZamestnanec { get; set; }
        public int IdZavod { get; set; }

        public Zamestnanec Zamestnanec { get; set; } = null!;
        public Zavod Zavod { get; set; } = null!;
    }
}