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
            // naète data z Oracle view V_ZAVODY_DETAIL
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
            // id = IdZavod, v reálu bys tu naèetl závod + aktuálnì pøihlášeného závodníka
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
    }
}
