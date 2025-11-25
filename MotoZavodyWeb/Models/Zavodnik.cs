namespace MotoZavodyWeb.Models
{
    public class Zavodnik
    {
        public int IdZavodnik { get; set; }

        public string Jmeno { get; set; } = string.Empty;

        public string Prijmeni { get; set; } = string.Empty;

        // může být NULL v DB → nullable
        public int? Vek { get; set; }
        // může být NULL v DB → nullable
        public string? Pohlavi { get; set; }           // 'M' / 'F'
                                                       // pro jistotu také nullable (kdyby v budoucnu bylo NULL)
        public string? UrovenZkusenosti { get; set; }

        public ICollection<Ucast> Ucasti { get; set; } = new List<Ucast>();
        public ICollection<JezdiNa> JezdiNa { get; set; } = new List<JezdiNa>();
    }
}