namespace MotoZavodyWeb.Models
{
    public class UzivatelProfilViewModel
    {
        public Uzivatel Uzivatel { get; set; } = null!;
        public Zavodnik? Zavodnik { get; set; }

        public List<UcastDetailView> NadchazejiciZavody { get; set; } = new();
        public List<UcastDetailView> MinuleZavody { get; set; } = new();

        public List<Kolobezka> Kolobezky { get; set; } = new();

        public decimal? CelkovaCastka { get; set; }
        public int PocetStartu { get; set; }

        // závody v budoucnu, na které ještě není přihlášen
        public List<ZavodDetailView> DostupneZavody { get; set; } = new();
    }
}
