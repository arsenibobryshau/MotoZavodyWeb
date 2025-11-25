using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoZavodyWeb.Data;
using MotoZavodyWeb.Models;

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
    }
}
