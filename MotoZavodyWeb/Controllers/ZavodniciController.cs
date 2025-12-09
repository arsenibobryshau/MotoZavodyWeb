using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoZavodyWeb.Data;
using MotoZavodyWeb.Models;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace MotoZavodyWeb.Controllers
{
    public class ZavodniciController : Controller
    {
        private readonly ZavodyContext _context;

        public ZavodniciController(ZavodyContext context)
        {
            _context = context;
        }

        // =====================================================
        // SEZNAM ZÁVODNÍKŮ
        // =====================================================
        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetInt32("UserId");

            bool maProfil = false;

            if (role == "USER" && userId != null)
            {
                var user = await _context.Uzivatele
                    .FirstOrDefaultAsync(u => u.IdUzivatel == userId);

                if (user != null)
                    maProfil = user.IdZavodnik != null;
            }

            ViewBag.MaProfil = maProfil;
            ViewBag.Role = role;

            var zavodnici = await _context.Zavodnici
                .OrderBy(z => z.Prijmeni)
                .ThenBy(z => z.Jmeno)
                .ToListAsync();

            return View(zavodnici);
        }


        // =====================================================
        // CREATE GET
        // =====================================================
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Uzivatele");

            var model = new ZavodnikCreateViewModel();

            if (role == "USER")
            {
                var uzivatel = _context.Uzivatele.First(u => u.IdUzivatel == userId);

                if (uzivatel.IdZavodnik != null)
                {
                    TempData["Error"] = "Již máte vytvořený závodnický profil.";
                    return RedirectToAction("Index");
                }

                model.Jmeno = uzivatel.Jmeno;
                model.Prijmeni = uzivatel.Prijmeni;
            }

            return View(model);
        }

        // =====================================================
        // CREATE POST
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ZavodnikCreateViewModel model)
        {
            var role = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Uzivatele");

            if (!ModelState.IsValid)
                return View(model);

            var pIdOut = new OracleParameter("p_id_zavodnik_out", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };

            string sql = @"BEGIN PR_REGISTRUJ_ZAVODNIKA(
                                :p_jmeno,
                                :p_prijmeni,
                                :p_vek,
                                :p_pohlavi,
                                :p_uroven,
                                :p_id_zavodnik_out
                           ); END;";

            await _context.Database.ExecuteSqlRawAsync(sql,
                new OracleParameter("p_jmeno", model.Jmeno),
                new OracleParameter("p_prijmeni", model.Prijmeni),
                new OracleParameter("p_vek", model.Vek),
                new OracleParameter("p_pohlavi", model.Pohlavi),
                new OracleParameter("p_uroven", model.UrovenZkusenosti),
                pIdOut
            );

            var oracleDecimal = (OracleDecimal)pIdOut.Value;
            int newId = oracleDecimal.ToInt32();

            if (role == "USER")
            {
                var uzivatel = _context.Uzivatele.Find(userId);
                uzivatel!.IdZavodnik = newId;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // EDIT (Bod 15)
        // =====================================================
        public async Task<IActionResult> Edit(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "ADMIN") return Unauthorized();

            var zavodnik = await _context.Zavodnici.FindAsync(id);
            if (zavodnik == null) return NotFound();

            return View(zavodnik);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Zavodnik model)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "ADMIN") return Unauthorized();

            if (id != model.IdZavodnik) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}