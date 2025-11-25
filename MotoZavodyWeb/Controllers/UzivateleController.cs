using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MotoZavodyWeb.Data;
using MotoZavodyWeb.Models;
using System.Security.Cryptography;
using System.Text;

namespace MotoZavodyWeb.Controllers
{
    public class UzivateleController : Controller
    {
        private readonly ZavodyContext _context;

        public UzivateleController(ZavodyContext context)
        {
            _context = context;
        }

        // Pomocná metoda pro hash hesla (SHA256)
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes); // např. "A1B2C3..."
        }

        // ================================
        //  GET: Registrace
        // ================================
        public IActionResult Registrace()
        {
            return View();
        }

        // ================================
        //  POST: Registrace (EF Core)
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registrace(string jmeno, string email, string heslo)
        {
            if (string.IsNullOrWhiteSpace(jmeno) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(heslo))
            {
                ViewBag.Error = "Vyplň všechna pole.";
                return View();
            }

            // kontrola duplicity emailu
            var existuje = _context.Uzivatele.Any(u => u.Email == email); //kvůli tomuto mi hází ERROR :D 
            if (existuje)
            {
                ViewBag.Error = "Uživatel s tímto e-mailem už existuje.";
                return View();
            }

            var hashed = HashPassword(heslo);

            var uzivatel = new Uzivatel
            {
                Jmeno = jmeno,
                Email = email,
                Heslo = hashed,
                Role = "USER",                
                DatumVytvoreni = DateTime.Now
            };

            _context.Uzivatele.Add(uzivatel);
            _context.SaveChanges();

            TempData["Success"] = "Registrace proběhla úspěšně. Nyní se můžete přihlásit.";
            return RedirectToAction("Login");
        }

        // ================================
        //  GET: Login
        // ================================
        public IActionResult Login()
        {
            if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"];
            }
            return View();
        }

        // ================================
        //  POST: Login
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string heslo)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(heslo))
            {
                ViewBag.Error = "Vyplň email i heslo.";
                return View();
            }

            var hashed = HashPassword(heslo);

            var uzivatel = _context.Uzivatele
                .FirstOrDefault(u => u.Email == email && u.Heslo == hashed);

            if (uzivatel == null)
            {
                ViewBag.Error = "Neplatné přihlašovací údaje.";
                return View();
            }

            // uložení do session
            HttpContext.Session.SetInt32("UserId", uzivatel.IdUzivatel);
            HttpContext.Session.SetString("UserRole", uzivatel.Role);
            HttpContext.Session.SetString("UserName", uzivatel.Jmeno);

            // update posledniho přihlášení (není nutné, ale je pěkné)
            uzivatel.DatumPoslednihoPrihlaseni = DateTime.Now;
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        // ================================
        //  Odhlášení
        // ================================
        public IActionResult Odhlasit()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
