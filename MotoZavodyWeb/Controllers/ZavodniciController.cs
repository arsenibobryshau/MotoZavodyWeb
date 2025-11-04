using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoZavodyWeb.Data;

namespace MotoZavodyWeb.Controllers
{
    public class ZavodniciController : Controller
    {
        private readonly ZavodyContext _context;

        public ZavodniciController(ZavodyContext context)
        {
            _context = context;
        }

        // GET: /Zavodnici/Profile/1
        public async Task<IActionResult> Profile(int id)
        {
            var zavodnik = await _context.Zavodnici
                .Include(z => z.JezdiNa)
                    .ThenInclude(j => j.Kolobezka)
                .FirstOrDefaultAsync(z => z.IdZavodnik == id);

            if (zavodnik == null)
                return NotFound();

            return View(zavodnik);
        }

        // GET: /Zavodnici/MyRaces/1
        public async Task<IActionResult> MyRaces(int id)
        {
            var ucasti = await _context.Ucasti
                .Include(u => u.Zavod)
                .Where(u => u.IdZavodnik == id)
                .OrderByDescending(u => u.Zavod.Datum)
                .ToListAsync();

            return View(ucasti);
        }
    }
}
