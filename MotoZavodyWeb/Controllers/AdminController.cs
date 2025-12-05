using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoZavodyWeb.Data;
using MotoZavodyWeb.Models;

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
        // Přehled uživatelů
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
            if (uzivatel == null)
            {
                return NotFound();
            }

            if (role != "USER" && role != "ADMIN")
            {
                // fallback – nic neuděláme
                return RedirectToAction("Uzivatele");
            }

            uzivatel.Role = role;
            _context.SaveChanges();

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
            if (uzivatel == null)
            {
                return NotFound();
            }

            // jednoduchý reset – nastavení hesla na "123456"
            // používáme stejnou hashovací funkci jako v UzivateleControlleru
            string noveHeslo = "123456";
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(noveHeslo));
            uzivatel.Heslo = Convert.ToHexString(bytes);

            _context.SaveChanges();

            TempData["Success"] = $"Uživateli {uzivatel.Email} bylo nastaveno nové heslo: {noveHeslo}";
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
            if (uzivatel == null)
            {
                return NotFound();
            }

            // pro jistotu nepovolíme smazat sám sebe
            if (CurrentUserId.HasValue && uzivatel.IdUzivatel == CurrentUserId.Value)
            {
                TempData["Error"] = "Nemůžeš smazat sám sebe.";
                return RedirectToAction("Uzivatele");
            }

            _context.Uzivatele.Remove(uzivatel);
            _context.SaveChanges();

            return RedirectToAction("Uzivatele");
        }
    }
}
