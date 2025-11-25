using System;
using MotoZavodyWeb.Models;

namespace MotoZavodyWeb.Models
{
    // DTO pro Oracle view V_ZAVODY_DETAIL
    public class ZavodDetailView
    {
        public int IdZavod { get; set; }
        public string NazevZavodu { get; set; } = string.Empty;
        public DateTime Datum { get; set; }
        public decimal Startovne { get; set; }

        public string TypZavodu { get; set; } = string.Empty;

        public string Misto { get; set; } = string.Empty;
        public string Ulice { get; set; } = string.Empty;
        public string Cp { get; set; } = string.Empty;
        public string Psc { get; set; } = string.Empty;
        public string Mesto { get; set; } = string.Empty;

        public string? MetodaHodnoceni { get; set; }
    }
}
