using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoZavodyWeb.Data;
using MotoZavodyWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Oracle.ManagedDataAccess.Client; // Nutné pro OracleParameter
using System.Data;

namespace MotoZavodyWeb.Controllers
{
    public class ZavodyController : Controller
    {
        private readonly ZavodyContext _context;

        public ZavodyController(ZavodyContext context)
        {
            _context = context;
        }

        // GET: /Zavody
        public async Task<IActionResult> Index()
        {
            var zavody = await _context.ZavodyDetail
                .OrderBy(z => z.Datum)
                .ToListAsync();

            return View(zavody);
        }

        // ---------------------------
        // DETAIL (S VÝSLEDKY A ORGANIZÁTORY)
        // ---------------------------  
        public async Task<IActionResult> Details(int id)
        {
            var zavod = await _context.Zavody
                .Include(z => z.TypZavodu)
                .Include(z => z.Misto)
                .Include(z => z.Hodnoceni)
                // Načtení organizátorů
                .Include(z => z.Organizatori)
                    .ThenInclude(o => o.Zamestnanec)
                    .ThenInclude(zam => zam.Pozice)
                // PŘIDÁNO: Načtení účastí a závodníků pro výsledkovou listinu
                .Include(z => z.Ucasti)
                    .ThenInclude(u => u.Zavodnik)
                .FirstOrDefaultAsync(z => z.IdZavod == id);

            if (zavod == null)
                return NotFound();

            // Funkce pro tržbu
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
                        trzba = Convert.ToDecimal(result);
                    }
                }
            }
            catch (Exception)
            {
                trzba = 0;
            }

            ViewBag.Trzba = trzba;

            return View(zavod);
        }

    

        // ---------------------------------------------
        // NOVÉ: PŘIDAT ORGANIZÁTORA (ZAMĚSTNANCE)
        // ---------------------------------------------
        public IActionResult AddOrganizer(int idZavod)
        {
            if (HttpContext.Session.GetString("UserRole") != "ADMIN")
                return Unauthorized();

            // Vybereme zaměstnance, kteří ještě NEJSOU přiřazeni k tomuto závodu
            var existujiciIds = _context.Organizace
                .Where(o => o.IdZavod == idZavod)
                .Select(o => o.IdZamestnanec)
                .ToList();

            var dostupniZamestnanci = _context.Zamestnanci
                .Include(z => z.Pozice)
                .Where(z => !existujiciIds.Contains(z.IdZamestnanec))
                .Select(z => new
                {
                    Id = z.IdZamestnanec,
                    Name = $"{z.Jmeno} {z.Prijmeni} ({z.Pozice.Nazev})"
                })
                .ToList();

            ViewBag.Zamestnanci = new SelectList(dostupniZamestnanci, "Id", "Name");
            ViewBag.IdZavod = idZavod;

            var model = new Organizace { IdZavod = idZavod };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrganizer(Organizace model)
        {
            if (HttpContext.Session.GetString("UserRole") != "ADMIN")
                return Unauthorized();

            // OPRAVA CHYBY "ORA-00904: FALSE":
            // Místo .AnyAsync() použijeme .CountAsync(), protože Oracle neumí v SQL boolean literály.
            int pocet = await _context.Organizace
                .CountAsync(o => o.IdZavod == model.IdZavod && o.IdZamestnanec == model.IdZamestnanec);

            bool existuje = pocet > 0;

            if (existuje)
            {
                // Už tam je, jen přesměrujeme
                return RedirectToAction("Details", new { id = model.IdZavod });
            }

            if (model.IdZamestnanec != 0) // Pokud byl vybrán zaměstnanec
            {
                _context.Organizace.Add(model);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = model.IdZavod });
        }

        // --- ZDE MUSÍŠ MÍT I ZBYTEK PŮVODNÍCH METOD (Edit, Delete, Register, Create) ---
        // Pokud si nejsi jistý, řekni a pošlu ti ZNOVU úplně celý soubor.
        // Ale předpokládám, že stačí přidat ty dvě metody AddOrganizer nakonec.

        // ---------------------------
        // REGISTRACE
        // ---------------------------        
        public IActionResult Register(int id)
        {
            var model = new Ucast { IdZavod = id };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Ucast model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.Ucasti.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = model.IdZavod });
        }

        // ---------------------------
        // CREATE
        // ---------------------------
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "ADMIN")
                return Unauthorized();

            ViewBag.Typy = _context.TypyZavodu
                .Select(t => new { t.IdTypZavodu, t.Nazev })
                .ToList();

            ViewBag.Mista = _context.Mista
                .Select(m => new { m.IdMisto, m.Nazev })
                .ToList();

            ViewBag.Hodnoceni = _context.Hodnoceni
                .Select(h => new { h.IdHodnoceni, h.Metoda })
                .ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Zavod model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Typy = _context.TypyZavodu.ToList();
                ViewBag.Mista = _context.Mista.ToList();
                ViewBag.Hodnoceni = _context.Hodnoceni.ToList();
                return View(model);
            }

            _context.Zavody.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ---------------------------
        // EDIT 
        // ---------------------------
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "ADMIN")
                return Unauthorized();

            var zavod = await _context.Zavody.FindAsync(id);
            if (zavod == null) return NotFound();

            ViewBag.Typy = _context.TypyZavodu
                 .Select(t => new { t.IdTypZavodu, t.Nazev })
                 .ToList();

            ViewBag.Mista = _context.Mista
                .Select(m => new { m.IdMisto, m.Nazev })
                .ToList();

            ViewBag.Hodnoceni = _context.Hodnoceni
                .Select(h => new { h.IdHodnoceni, h.Metoda })
                .ToList();

            return View(zavod);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Zavod model)
        {
            if (HttpContext.Session.GetString("UserRole") != "ADMIN")
                return Unauthorized();

            if (id != model.IdZavod) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Zavody.Any(e => e.IdZavod == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Typy = _context.TypyZavodu.ToList();
            ViewBag.Mista = _context.Mista.ToList();
            ViewBag.Hodnoceni = _context.Hodnoceni.ToList();
            return View(model);
        }

        // ---------------------------
        // DELETE 
        // ---------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "ADMIN")
                return Unauthorized();

            var zavod = await _context.Zavody.FindAsync(id);
            if (zavod != null)
            {
                _context.Zavody.Remove(zavod);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}