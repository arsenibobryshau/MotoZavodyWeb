using System.ComponentModel.DataAnnotations;

namespace MotoZavodyWeb.Models
{
    public class Zavodnik
    {
        public int IdZavodnik { get; set; }

        [Required(ErrorMessage = "Zadej jméno.")]
        public string Jmeno { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zadej příjmení.")]
        public string Prijmeni { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zadej věk.")]
        [Range(5, 100, ErrorMessage = "Věk musí být mezi 5 a 100 lety.")]
        public int? Vek { get; set; }

        [Required(ErrorMessage = "Vyber pohlaví.")]
        public string? Pohlavi { get; set; }           // 'M' / 'F'
                                                       // pro jistotu také nullable (kdyby v budoucnu bylo NULL)
        [Required(ErrorMessage = "Vyber úroveň zkušeností.")]
        public string? UrovenZkusenosti { get; set; }

        public ICollection<Ucast> Ucasti { get; set; } = new List<Ucast>();
        public ICollection<JezdiNa> JezdiNa { get; set; } = new List<JezdiNa>();
    }
}