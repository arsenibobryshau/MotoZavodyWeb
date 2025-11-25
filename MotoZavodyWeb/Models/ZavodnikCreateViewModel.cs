using System.ComponentModel.DataAnnotations;

namespace MotoZavodyWeb.Models
{
    public class ZavodnikCreateViewModel
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "Jméno")]
        public string Jmeno { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Příjmení")]
        public string Prijmeni { get; set; } = string.Empty;

        [Required]
        [Range(10, 120, ErrorMessage = "Věk musí být mezi 10 a 120.")]
        [Display(Name = "Věk")]
        public int Vek { get; set; }

        [Required]
        [Display(Name = "Pohlaví")]
        public string Pohlavi { get; set; } = "M";   // M / Z

        [Required]
        [Display(Name = "Úroveň zkušenosti")]
        public string UrovenZkusenosti { get; set; } = "A"; // A–D
    }
}
