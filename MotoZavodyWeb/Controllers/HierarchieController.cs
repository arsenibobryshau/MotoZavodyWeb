using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoZavodyWeb.Data;

namespace MotoZavodyWeb.Controllers
{
    public class HierarchieController : Controller
    {
        private readonly ZavodyContext _context;

        public HierarchieController(ZavodyContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Načte data z pohledu V_ZAVODY_HIERARCHIE
            // Očekáváme, že Oracle View už obsahuje CONNECT BY logiku
            var data = await _context.Hierarchie.ToListAsync();
            return View(data);
        }
    }
}