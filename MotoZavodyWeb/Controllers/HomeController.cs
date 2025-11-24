using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoZavodyWeb.Data;

namespace MotoZavodyWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ZavodyContext _context;

        public HomeController(ZavodyContext context)
        {
            _context = context;
        }

        //public async Task<IActionResult> Index()
        //{
        //    // Nadcházející závody (dnes a dál)
        //    var today = DateTime.Today;

        //    var zavody = await _context.Zavody
        //        .Where(z => z.Datum >= today)
        //        .OrderBy(z => z.Datum)
        //        .Take(10)
        //        .ToListAsync();

        //    return View(zavody);
        //}

        public IActionResult TestDb()
        {
            try
            {
                var count = _context.Zavody.Count(); // Pouzije tabulku Zavody
                return Content($"DB OK! Záznamù v ZAVODY: {count}");
            }
            catch (Exception ex)
            {
                return Content("Chyba DB: " + ex.Message);
            }
        }
    }

}