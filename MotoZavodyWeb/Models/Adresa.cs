namespace MotoZavodyWeb.Models
{
    public class Adresa
    {
        public int IdAdresa { get; set; }
        public string Ulice { get; set; } = null!;
        public int Cp { get; set; }

        public int IdPosta { get; set; }
        public Posta Posta { get; set; } = null!;

        public ICollection<Misto> Mista { get; set; } = new List<Misto>();
    }
}