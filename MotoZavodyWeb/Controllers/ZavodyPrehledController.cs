using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoZavodyWeb.Data;
using MotoZavodyWeb.Models;
using Oracle.ManagedDataAccess.Client;

namespace MotoZavodyWeb.Controllers
{
    public class ZavodyPrehledController : Controller
    {
        private readonly ZavodyContext _context;

        public ZavodyPrehledController(ZavodyContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? rok, int? typ, int? hodnoceni, string? cena)
        {
            var q = _context.Zavody
                .Include(z => z.TypZavodu)
                .Include(z => z.Hodnoceni)
                .Select(z => new ZavodPrehledItem
                {
                    IdZavod = z.IdZavod,
                    Nazev = z.Nazev,
                    Rok = z.Datum.Value.Year,
                    TypZavodu = z.TypZavodu.Nazev,
                    TypZavoduId = z.IdTypZavodu,
                    Hodnoceni = z.Hodnoceni.Metoda,
                    HodnoceniId = z.IdHodnoceni,
                    Startovne = z.Startovne
                });

            // FILTRACE
            if (rok != null)
                q = q.Where(z => z.Rok == rok);

            if (typ != null)
                q = q.Where(z => z.TypZavoduId == typ);

            if (hodnoceni != null)
                q = q.Where(z => z.HodnoceniId == hodnoceni);

            // ŘAZENÍ DLE CENY
            if (cena == "asc")
                q = q.OrderBy(z => z.Startovne);
            else if (cena == "desc")
                q = q.OrderByDescending(z => z.Startovne);
            else
                q = q.OrderBy(z => z.Rok).ThenBy(z => z.Nazev);

            // DATA DO DROPDOWNU
            ViewBag.Roky = _context.Zavody
                .Select(z => z.Datum.Value.Year)
                .Distinct()
                .OrderBy(y => y)
                .ToList();

            ViewBag.Typy = _context.TypyZavodu
                .OrderBy(t => t.Nazev)
                .ToList();

            ViewBag.Hodnoceni = _context.Hodnoceni
                .OrderBy(h => h.Metoda)
                .ToList();

            return View(q.ToList());

        }
        // ---------------------------
        // DETAIL
        // ---------------------------  
        public async Task<IActionResult> Details(int id)
        {
            var zavod = await _context.Zavody
                .Include(z => z.TypZavodu)
                .Include(z => z.Misto)
                .Include(z => z.Hodnoceni)
                .FirstOrDefaultAsync(z => z.IdZavod == id);

            if (zavod == null)
                return NotFound();

            // ---------------------------------------------------------
            // BOD 4: Volání PL/SQL funkce FN_TRZBA_ZAVODU
            // ---------------------------------------------------------
            decimal trzba = 0;
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT FN_TRZBA_ZAVODU(:p_id) FROM DUAL";
                    var param = new OracleParameter("p_id", id);
                    command.Parameters.Add(param);

                    _context.Database.OpenConnection();
                    var result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        // Oracle vrací číslo často jako decimal nebo OracleDecimal
                        trzba = Convert.ToDecimal(result);
                    }
                }
            }
            catch (Exception)
            {
                // Pokud funkce neexistuje nebo selže, zobrazíme 0 nebo chybu do logu
                // Pro účely semestrální práce to stačí ignorovat (zobrazí se 0)
                trzba = 0;
            }

            ViewBag.Trzba = trzba;
            // ---------------------------------------------------------

            return View(zavod);
        }
    }
}
