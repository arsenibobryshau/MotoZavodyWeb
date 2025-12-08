using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MotoZavodyWeb.Models
{
    public class DokumentCreateViewModel
    {
        [Required]
        [Display(Name = "Závodník")]
        public int IdZavodnik { get; set; }

        [Required]
        [Display(Name = "Soubor")]
        public IFormFile Soubor { get; set; } = default!;

        public bool ViditelneVsem { get; set; }   

        public IEnumerable<SelectListItem> Zavodnici { get; set; } = new List<SelectListItem>();
    }
}
