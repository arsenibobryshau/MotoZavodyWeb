namespace MotoZavodyWeb.Models
{
    public class Platba
    {
        public int IdPlatby { get; set; }
        public decimal Castka { get; set; }
        public DateTime Datum { get; set; }
        public string Typ { get; set; } = null!; // HOTOVE/KARTOU

        public ICollection<Ucast> Ucasti { get; set; } = new List<Ucast>();
    }
}