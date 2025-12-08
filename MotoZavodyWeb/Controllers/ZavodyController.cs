using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoZavodyWeb.Data;
using MotoZavodyWeb.Models;
    
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

            return View(zavod);
        }

        // GET: /Zavody/Register/5
        public IActionResult Register(int id)
        {
            // id = IdZavod, v reálu bys tu načetl závod + aktuálně přihlášeného závodníka
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

        // GET: /Zavody/Create
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


        // POST: /Zavody/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Zavod model)
        {
            if (!ModelState.IsValid)
            {
                // ← MUSÍŠ DOPLNIT, aby selecty nebyly NULL
                ViewBag.Typy = _context.TypyZavodu.ToList();
                ViewBag.Mista = _context.Mista.ToList();
                ViewBag.Hodnoceni = _context.Hodnoceni.ToList();

                return View(model);
            }

            _context.Zavody.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



    }
}
