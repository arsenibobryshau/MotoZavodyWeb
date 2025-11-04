using Microsoft.AspNetCore.Mvc;
using MotoZavodyWeb.Data;
using MotoZavodyWeb.Models;

namespace MotoZavodyWeb.Controllers
{
    public class PlatbyController : Controller
    {
        private readonly ZavodyContext _context;

        public PlatbyController(ZavodyContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create(int idZavodnik, int idZavod, decimal castka)
        {
            ViewBag.IdZavodnik = idZavodnik;
            ViewBag.IdZavod = idZavod;
            ViewBag.Castka = castka;

            var model = new Platba
            {
                Castka = castka,
                Datum = DateTime.Today
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int idZavodnik, int idZavod, Platba platba)
        {
            if (!ModelState.IsValid)
                return View(platba);

            // uložíme platbu
            _context.Platby.Add(platba);
            await _context.SaveChangesAsync();

            // navážeme na úèast
            var ucast = await _context.Ucasti.FindAsync(idZavodnik, idZavod);
            if (ucast != null)
            {
                ucast.IdPlatby = platba.IdPlatby;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Zavody", new { id = idZavod });
        }
    }
}
