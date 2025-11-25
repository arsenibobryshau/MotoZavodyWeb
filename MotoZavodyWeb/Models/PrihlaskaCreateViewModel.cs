using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MotoZavodyWeb.Models
{
    public class PrihlaskaCreateViewModel
    {
        [Required]
        [Display(Name = "Závodník")]
        public int IdZavodnik { get; set; }

        [Required]
        [Display(Name = "Závod")]
        public int IdZavod { get; set; }

        [Required]
        [Range(0, 100000, ErrorMessage = "Částka musí být nezáporná.")]
        [Display(Name = "Částka")]
        public decimal Castka { get; set; }

        [Required]
        [Display(Name = "Typ platby")]
        public string TypPlatby { get; set; } = "H";  // H = hotově, K = kartou

        [Display(Name = "Číslo karty")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Číslo karty musí obsahovat přesně 16 číslic.")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Číslo karty může obsahovat pouze číslice.")]
        public string? CisloKarty { get; set; }

        // pro comboboxy
        public IEnumerable<SelectListItem> Zavodnici { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Zavody { get; set; } = new List<SelectListItem>();
        public Dictionary<int, decimal> StartovneDict { get; set; } = new();

    }
}
