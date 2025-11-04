namespace MotoZavodyWeb.Models
{
    public class Zamestnanec
    {
        public int IdZamestnanec { get; set; }
        public string Jmeno { get; set; } = null!;
        public string Prijmeni { get; set; } = null!;

        public int IdPozice { get; set; }
        public Pozice Pozice { get; set; } = null!;

        public ICollection<Organizace> OrganizovaneZavody { get; set; } = new List<Organizace>();
    }
}