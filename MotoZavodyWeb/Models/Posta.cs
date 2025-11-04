namespace MotoZavodyWeb.Models
{
    public class Posta
    {
        public int IdPosta { get; set; }
        public string Psc { get; set; } = null!;
        public string Mesto { get; set; } = null!;

        public ICollection<Adresa> Adresy { get; set; } = new List<Adresa>();
    }
}