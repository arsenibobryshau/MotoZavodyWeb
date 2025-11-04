using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoZavodyWeb.Data;

namespace MotoZavodyWeb.Controllers
{
    public class OrganizaceController : Controller
    {
        private readonly ZavodyContext _context;

        public OrganizaceController(ZavodyContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var org = await _context.Organizace
                .Include(o => o.Zamestnanec).ThenInclude(z => z.Pozice)
                .Include(o => o.Zavod)
                .ToListAsync();

            return View(org);
        }
    }
}
