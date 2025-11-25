using System;

namespace MotoZavodyWeb.Models
{
    public class UcastDetailView
    {
        public int IdZavodnik { get; set; }
        public int IdZavod { get; set; }

        public string Jmeno { get; set; } = string.Empty;
        public string Prijmeni { get; set; } = string.Empty;

        public string NazevZavodu { get; set; } = string.Empty;
        public DateTime DatumZavodu { get; set; }

        public decimal Castka { get; set; }
        public string TypPlatby { get; set; } = string.Empty;
    }
}
