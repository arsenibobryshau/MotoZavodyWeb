namespace MotoZavodyWeb.Models
{
    public class Uzivatel
    {
        public int IdUzivatel { get; set; }
        public string Jmeno { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Heslo { get; set; } = string.Empty;
        public string Role { get; set; } = "USER";   // Anon nebude v DB
        public DateTime DatumVytvoreni { get; set; }
        public DateTime? DatumPoslednihoPrihlaseni { get; set; }
    }
}
