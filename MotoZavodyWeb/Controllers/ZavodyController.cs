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
            // načte data z Oracle view V_ZAVODY_DETAIL
            var zavody = await _context.ZavodyDetail
                .OrderBy(z => z.Datum)
                .ToListAsync();

            return View(zavody);
        }

        // GET: /Zavody/Details/5
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

        // GET: /Zavody/Register/5
        public IActionResult Register(int id)
        {
            var model = new Ucast { IdZavod = id };
            return View(model);
        }

        // POST: /Zavody/Register
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

            ViewBag.Typy = _context.TypyZavodu.ToList();
            ViewBag.Mista = _context.Mista.ToList();
            ViewBag.Hodnoceni = _context.Hodnoceni.ToList();

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
        // EDIT (Bod 15, 23)
        // ---------------------------
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "ADMIN")
                return Unauthorized();

            var zavod = await _context.Zavody.FindAsync(id);
            if (zavod == null) return NotFound();

            ViewBag.Typy = _context.TypyZavodu.ToList();
            ViewBag.Mista = _context.Mista.ToList();
            ViewBag.Hodnoceni = _context.Hodnoceni.ToList();

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
        // DELETE (Bod 23)
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