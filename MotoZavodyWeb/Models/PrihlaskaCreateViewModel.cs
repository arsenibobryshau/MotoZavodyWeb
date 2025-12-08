using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using MotoZavodyWeb.Models.Validation;

namespace MotoZavodyWeb.Models
{
    public class PrihlaskaCreateViewModel
    {
        [Required]
        [Display(Name = "Závodník")]
        public int IdZavodnik { get; set; }

        // Jméno závodníka pro zobrazení, když si přihlašuje sám sebe
        public string? JmenoZavodnika { get; set; }

        [Required]
        [Display(Name = "Závod")]
        public int IdZavod { get; set; }

        [Required]
        [Range(0, 100000, ErrorMessage = "Částka musí být nezáporná.")]
        [Display(Name = "Částka")]
        public decimal Castka { get; set; }

        [Required]
        [Display(Name = "Typ platby")]
        public string TypPlatby { get; set; } = "H";

        [Display(Name = "Číslo karty")]
        [RequiredIfCardPayment]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Číslo karty musí obsahovat přesně 16 číslic.")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Číslo karty může obsahovat pouze číslice.")]
        public string? CisloKarty { get; set; }

        // comboboxy
        public IEnumerable<SelectListItem> Zavodnici { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Zavody { get; set; } = new List<SelectListItem>();
        public Dictionary<int, decimal> StartovneDict { get; set; } = new();
    }
}
