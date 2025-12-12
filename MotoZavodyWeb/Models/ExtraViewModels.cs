using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotoZavodyWeb.Models
{
    // Model pro Bod 17 (Hierarchický dotaz)
    // Mapuje se na View V_ZAVODY_HIERARCHIE
    public class HierarchieItem
    {
        // Předpokládám, že view vrací textový výpis už odsazený nebo s číslem úrovně
        // Pokud view vrací sloupec "TEXT", "NAZEV" nebo podobně
        [Column("NAZEV")]
        public string Text { get; set; } = string.Empty;

        [Column("UROVEN")]
        public int Uroven { get; set; }
    }

    // Model pro Bod 21 (Historie/Logy)
    // Mapuje se na tabulku PLATBY_LOG (dle tvého screenshotu)
    public class LogPolozka
    {
        [Column("ID_LOG")]
        public int IdLog { get; set; }

        [Column("DATUM_OPERACE")]
        public DateTime Datum { get; set; }

        [Column("UZIVATEL")]
        public string Uzivatel { get; set; } = string.Empty;

        [Column("OPERACE")]
        public string Operace { get; set; } = string.Empty; // INSERT/UPDATE/DELETE

        [Column("CASTKA")]
        public decimal? Castka { get; set; }

        [Column("STARA_CASTKA")]
        public decimal? StaraCastka { get; set; }

        [Column("NOVA_CASTKA")]
        public decimal? NovaCastka { get; set; }

        [Column("ID_PLATBY")]
        public int IdPlatby { get; set; }
    }
    //Model pro Historii Závodníků (ZAVODNICI_HIST) 
    public class ZavodnikHistLog
    {
        [Column("ID_HIST")]
        public int IdHist { get; set; }

        [Column("ID_ZAVODNIK")]
        public int IdZavodnik { get; set; }

        [Column("JMENO")]
        public string Jmeno { get; set; }

        [Column("PRIJMENI")]
        public string Prijmeni { get; set; }

        [Column("VEK")]
        public int? Vek { get; set; } // Nullable, v DB může být null

        [Column("POHLAVI")]
        public string Pohlavi { get; set; }

        [Column("UROVEN_ZKUSENOSTI")]
        public string UrovenZkusenosti { get; set; }

        [Column("OPERACE")]
        public string Operace { get; set; }

        [Column("DATUM_OPERACE")]
        public DateTime DatumOperace { get; set; }

        [Column("UZIVATEL")]
        public string Uzivatel { get; set; }
    }

    //ViewModel pro zobrazení obou tabulek najednou
    public class AuditLogViewModel
    {
        public IEnumerable<LogPolozka> LogyPlateb { get; set; }
        public IEnumerable<ZavodnikHistLog> LogyZavodniku { get; set; }
    }
}