using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace MotoZavodyWeb.Models
{
    public class Zavod
    {
        public int IdZavod { get; set; }
        public string Nazev { get; set; } = null!;
        public DateTime? Datum { get; set; }
        public decimal Startovne { get; set; }

        public int IdTypZavodu { get; set; }
        public int IdMisto { get; set; }
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
