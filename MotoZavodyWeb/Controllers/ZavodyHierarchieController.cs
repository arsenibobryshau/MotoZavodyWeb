using Microsoft.AspNetCore.Mvc;
using MotoZavodyWeb.Data;
using MotoZavodyWeb.Models;

namespace MotoZavodyWeb.Controllers
{
    public class ZavodyHierarchieController : Controller
    {
        private readonly ZavodyContext _context;

        public ZavodyHierarchieController(ZavodyContext context)
        {
            _context = context;
        }

        // GET: /ZavodyHierarchie
        public IActionResult Index()
        {
            // Načteme všechno z view a v Razor si to srovnáme
            var data = _context.ZavodyHierarchie
                .OrderBy(h => h.Rok)
                .ThenBy(h => h.Uroven)
                .ThenBy(h => h.Nazev)
                .ToList();

            return View(data);
        }
    }
}
