namespace MotoZavodyWeb.Models
{
    public class Misto
    {
        public int IdMisto { get; set; }
        public string Nazev { get; set; } = null!;

        public int IdAdresa { get; set; }
        public Adresa Adresa { get; set; } = null!; /*- jebka to cely db*/

        public ICollection<Zavod> Zavody { get; set; } = new List<Zavod>();
    }
}