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
            // 🔒 ZABEZPEČENÍ: Přístup pouze pro ADMINA
            if (HttpContext.Session.GetString("UserRole") != "ADMIN")
            {
                return Unauthorized(); // Nebo RedirectToAction("Index", "Home");
            }

            // Načte data z pohledu V_ZAVODY_HIERARCHIE
            var data = await _context.Hierarchie.ToListAsync();
            return View(data);
        }
    }
}