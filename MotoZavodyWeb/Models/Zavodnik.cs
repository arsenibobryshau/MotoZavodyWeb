namespace MotoZavodyWeb.Models
{
    public class Zavodnik
    {
        public int IdZavodnik { get; set; }
        public string Jmeno { get; set; } = null!;
        public string Prijmeni { get; set; } = null!;
        public int Vek { get; set; }
        public char Pohlavi { get; set; }           // 'M' / 'F'
        public char UrovenZkusenosti { get; set; }  // 'A', 'B', 'C'...

        public ICollection<Ucast> Ucasti { get; set; } = new List<Ucast>();
        public ICollection<JezdiNa> JezdiNa { get; set; } = new List<JezdiNa>();
    }
}