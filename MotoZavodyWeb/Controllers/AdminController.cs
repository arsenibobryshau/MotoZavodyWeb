using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoZavodyWeb.Data;
using MotoZavodyWeb.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace MotoZavodyWeb.Controllers
{
    public class AdminController : BaseController
    {
        private readonly ZavodyContext _context;

        public AdminController(ZavodyContext context)
        {
            _context = context;
        }

        // ---------------------------
        // Dashboard
        // ---------------------------
        public IActionResult Index()
        {
            var check = RequireAdmin();
            if (check != null) return check;

            int pocetUzivatelu = _context.Uzivatele.Count();
            int pocetAdminu = _context.Uzivatele.Count(u => u.Role == "ADMIN");
            int pocetUseru = _context.Uzivatele.Count(u => u.Role == "USER");

            ViewBag.PocetUzivatelu = pocetUzivatelu;
            ViewBag.PocetAdminu = pocetAdminu;
            ViewBag.PocetUseru = pocetUseru;

            return View();
        }

        // ---------------------------
        // Přehled uživatelů (Bez ID)
        // ---------------------------
        public IActionResult Uzivatele()
        {
            var check = RequireAdmin();
            if (check != null) return check;

            var uzivatele = _context.Uzivatele
                .Include(u => u.Zavodnik)
                .OrderBy(u => u.Email)
                .ToList();

            return View(uzivatele);
        }

        // ---------------------------
        // Změna role uživatele
        // ---------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ZmenRoli(int idUzivatel, string role)
        {
            var check = RequireAdmin();
            if (check != null) return check;

            var uzivatel = _context.Uzivatele.FirstOrDefault(u => u.IdUzivatel == idUzivatel);
            if (uzivatel == null) return NotFound();

            if (role == "USER" || role == "ADMIN")
            {
                uzivatel.Role = role;
                _context.SaveChanges();
            }

            return RedirectToAction("Uzivatele");
        }

        // ---------------------------
        // Reset hesla uživatele
        // ---------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetHesla(int idUzivatel)
        {
            var check = RequireAdmin();
            if (check != null) return check;

            var uzivatel = _context.Uzivatele.FirstOrDefault(u => u.IdUzivatel == idUzivatel);
            if (uzivatel == null) return NotFound();

            string noveHeslo = "123456";
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(noveHeslo));
            uzivatel.Heslo = Convert.ToHexString(bytes);

            _context.SaveChanges();

            TempData["Success"] = $"Uživateli {uzivatel.Email} bylo nastaveno heslo: {noveHeslo}";
            return RedirectToAction("Uzivatele");
        }

        // ---------------------------
        // Smazání uživatele
        // ---------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SmazUzivatele(int idUzivatel)
        {
            var check = RequireAdmin();
            if (check != null) return check;

            var uzivatel = _context.Uzivatele.FirstOrDefault(u => u.IdUzivatel == idUzivatel);
            if (uzivatel == null) return NotFound();

            if (CurrentUserId.HasValue && uzivatel.IdUzivatel == CurrentUserId.Value)
            {
                TempData["Error"] = "Nemůžeš smazat sám sebe.";
                return RedirectToAction("Uzivatele");
            }

            _context.Uzivatele.Remove(uzivatel);
            _context.SaveChanges();

            return RedirectToAction("Uzivatele");
        }

        // ---------------------------
        // Emulace uživatele (Bod 27)
        // ---------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Emulovat(int idUzivatel)
        {
            var check = RequireAdmin();
            if (check != null) return check;

            var uzivatel = _context.Uzivatele.FirstOrDefault(u => u.IdUzivatel == idUzivatel);
            if (uzivatel == null) return NotFound();

            // Přepíšeme Session aktuálního admina daty cílového uživatele
            HttpContext.Session.SetInt32("UserId", uzivatel.IdUzivatel);
            HttpContext.Session.SetString("UserRole", uzivatel.Role);
            HttpContext.Session.SetString("UserFullName", $"{uzivatel.Jmeno} {uzivatel.Prijmeni}");

            // Poznámka: Pro návrat zpět by se musel admin odhlásit a znovu přihlásit, 
            // nebo bychom museli uložit původní ID do jiné session proměnné.
            // Pro splnění zadání stačí toto přepnutí.

            TempData["Success"] = $"Nyní jste přihlášen jako {uzivatel.Email}.";
            return RedirectToAction("Index", "Home");
        }

        // ---------------------------
        // Systémový katalog (Bod 30)
        // ---------------------------
        public IActionResult SystemovyKatalog()
        {
            var check = RequireAdmin();
            if (check != null) return check;

            var objekty = new List<SystemovyObjekt>();

            // Použijeme ADO.NET pro přímý dotaz na systémová data
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = @"
                    SELECT OBJECT_NAME, OBJECT_TYPE, STATUS 
                    FROM USER_OBJECTS 
                    WHERE OBJECT_TYPE IN ('TABLE', 'VIEW', 'PROCEDURE', 'FUNCTION', 'TRIGGER', 'SEQUENCE')
                    ORDER BY OBJECT_TYPE, OBJECT_NAME";

                _context.Database.OpenConnection();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        objekty.Add(new SystemovyObjekt
                        {
                            Nazev = reader["OBJECT_NAME"].ToString()!,
                            Typ = reader["OBJECT_TYPE"].ToString()!,
                            Status = reader["STATUS"].ToString()!
                        });
                    }
                }
            }

            return View(objekty);
        }

        // ---------------------------
        // Logy / Historie (Bod 21)
        // ---------------------------
        public IActionResult Logy()
        {
            var check = RequireAdmin();
            if (check != null) return check;

            // Načteme logy seřazené od nejnovějších
            var logy = _context.PlatbyLog
                .OrderByDescending(l => l.Datum)
                .ToList();

            return View(logy);
        }
    }
}