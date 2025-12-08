using System;

namespace MotoZavodyWeb.Models
{
    public class DokumentZavodnika
    {
        public int IdDokument { get; set; }
        public int IdZavodnik { get; set; }

        public string NazevSouboru { get; set; } = string.Empty;
        public string TypSouboru { get; set; } = string.Empty;
        public string PriponaSouboru { get; set; } = string.Empty;

        public byte[] Obsah { get; set; } = Array.Empty<byte>();

        public DateTime? DatumNahrani { get; set; }
        public DateTime? DatumModifikace { get; set; }
        public string? UzivatelVytvoril { get; set; }
        public string? UzivatelZmenil { get; set; }

        public Zavodnik? Zavodnik { get; set; }

        public bool ViditelneVsem { get; set; }
    }
}
