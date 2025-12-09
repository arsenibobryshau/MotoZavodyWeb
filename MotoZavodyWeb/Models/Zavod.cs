using MotoZavodyWeb.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MotoZavodyWeb.Models
{
    public class Zavod
    {
        public int IdZavod { get; set; }

        [Display(Name = "Název závodu")]
        [Required(ErrorMessage = "Název závodu je povinný.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Název musí mít alespoò 3 znaky.")]
        public string Nazev { get; set; } = null!;

        [Display(Name = "Datum závodu")]
        [Required(ErrorMessage = "Datum závodu je povinné.")]
        [DateRange]
        public DateTime? Datum { get; set; }

        [Display(Name = "Startovné")]
        [Range(1, 20000, ErrorMessage = "Startovné musí být mezi 1 a 20 000 Kè.")]
        public decimal Startovne { get; set; }

        [Display(Name = "Typ závodu")]
        [Required(ErrorMessage = "Vyberte typ závodu.")]
        public int IdTypZavodu { get; set; }

        [Display(Name = "Místo")]
        [Required(ErrorMessage = "Vyberte místo konání.")]
        public int IdMisto { get; set; }

        [Display(Name = "Hodnocení")]
        [Required(ErrorMessage = "Vyberte hodnocení.")]
        public int IdHodnoceni { get; set; }

        [ForeignKey(nameof(IdTypZavodu))]
        public TypZavodu? TypZavodu { get; set; }

        [ForeignKey(nameof(IdMisto))]
        public Misto? Misto { get; set; }

        [ForeignKey(nameof(IdHodnoceni))]
        public Hodnoceni? Hodnoceni { get; set; }

        public ICollection<Ucast> Ucasti { get; set; } = new List<Ucast>();
        public ICollection<Organizace> Organizatori { get; set; } = new List<Organizace>();
    }

}
