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

        // /  nebo /Home/Index
        public IActionResult Index()
        {
            return View();
        }

        // p˘vodnÌ test DB ñ nech·me ho, aù m˘ûeö kontrolovat p¯ipojenÌ
        // GET: /Home/TestDb
        public async Task<IActionResult> TestDb()
        {
            var count = await _context.Zavody.CountAsync();
            return Content($"DB OK! Z·znam˘ v ZAVODY: {count}");
        }
    }
}
