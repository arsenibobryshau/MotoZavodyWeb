using System;
using System.Collections.Generic;

namespace MotoZavodyWeb.Models
{
    public class Zavod
    {
        public int IdZavod { get; set; }
        public string Nazev { get; set; } = null!;
        public DateTime Datum { get; set; }
        public decimal Startovne { get; set; }

        public int IdTypZavodu { get; set; }
        public int IdMisto { get; set; }
        public int IdHodnoceni { get; set; }

        public TypZavodu TypZavodu { get; set; } = null!;
        public Misto Misto { get; set; } = null!;
        public Hodnoceni Hodnoceni { get; set; } = null!;

        public ICollection<Ucast> Ucasti { get; set; } = new List<Ucast>();
        public ICollection<Organizace> Organizatori { get; set; } = new List<Organizace>();
    }
}
