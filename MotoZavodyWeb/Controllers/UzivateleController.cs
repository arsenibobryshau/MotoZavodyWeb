using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoZavodyWeb.Data;
using MotoZavodyWeb.Models;
using System.Security.Cryptography;
using System.Text;

namespace MotoZavodyWeb.Controllers
{
    public class UzivateleController : BaseController
    {
        private readonly ZavodyContext _context;

        public UzivateleController(ZavodyContext context)
        {
            _context = context;
        }

        // ===========================================
        //              Pomocná metoda
        // ===========================================
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }

        // ===========================================
        //                  REGISTRACE
        // ===========================================
        [HttpGet]
        public IActionResult Registrace()
        {
            return View();
        }

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

            var existujici = _context.Uzivatele
                .FirstOrDefault(u => u.Email == email);

            if (existujici != null)
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

        // ===========================================
        //                     LOGIN
        // ===========================================
        [HttpGet]
        public IActionResult Login()
        {
            if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"];
            }

            return View();
        }

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

            HttpContext.Session.SetInt32("UserId", uzivatel.IdUzivatel);
            HttpContext.Session.SetString("UserRole", uzivatel.Role);
            HttpContext.Session.SetString("UserName", uzivatel.Jmeno);

            uzivatel.DatumPoslednihoPrihlaseni = DateTime.Now;
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        // ===========================================
        //                  ODHLÁŠENÍ
        // ===========================================
        public IActionResult Odhlasit()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // ===========================================
        //                  PROFIL
        // ===========================================
        [HttpGet]
        public IActionResult Profil()
        {
            var check = RequireLogin();
            if (check != null) return check;

            var userId = CurrentUserId!.Value;

            var uzivatel = _context.Uzivatele
                .Include(u => u.Zavodnik)
                .FirstOrDefault(u => u.IdUzivatel == userId);

            if (uzivatel == null)
            {
                return RedirectToAction("Login");
            }

            int? idZavodnik = uzivatel.IdZavodnik;
            var dnes = DateTime.Today;

            List<UcastDetailView> nadchazejici = new();
            List<UcastDetailView> minule = new();
            List<Kolobezka> kolobezky = new();
            decimal? celkovaCastka = null;
            int pocetStartu = 0;
            List<ZavodDetailView> dostupneZavody = new();

            if (idZavodnik.HasValue)
            {
                var ucasti = _context.UcastiDetail
                    .Where(u => u.IdZavodnik == idZavodnik.Value)
                    .ToList();

                nadchazejici = ucasti
                    .Where(u => u.DatumZavodu >= dnes)
                    .OrderBy(u => u.DatumZavodu)
                    .ToList();

                minule = ucasti
                    .Where(u => u.DatumZavodu < dnes)
                    .OrderByDescending(u => u.DatumZavodu)
                    .ToList();

                celkovaCastka = ucasti.Sum(u => u.Castka);
                pocetStartu = ucasti.Count;

                // ===== OPRAVENÝ DOTAZ NA KOLOBĚŽKY =====
                // Vytváříme novou instanci Kolobezka z existujících sloupců,
                // takže EF nepřidá do SQL žádný neexistující sloupec.
                kolobezky = _context.JezdiNa
                    .Where(j => j.IdZavodnik == idZavodnik.Value)
                    .Join(
                        _context.Kolobezky,
                        j => j.IdKolobezka,
                        k => k.IdKolobezka,
                        (j, k) => new Kolobezka
                        {
                            IdKolobezka = k.IdKolobezka,
                            Model = k.Model,
                            Znacka = k.Znacka,
                            IdTypKolobezky = k.IdTypKolobezky
                        }
                    )
                    .ToList();

                var uzPrihlasenIds = ucasti
                    .Select(u => u.IdZavod)
                    .Distinct()
                    .ToList();

                dostupneZavody = _context.ZavodyDetail
                    .Where(z => z.Datum >= dnes &&
                                !uzPrihlasenIds.Contains(z.IdZavod))
                    .OrderBy(z => z.Datum)
                    .ToList();
            }

            var vm = new UzivatelProfilViewModel
            {
                Uzivatel = uzivatel,
                Zavodnik = uzivatel.Zavodnik,
                NadchazejiciZavody = nadchazejici,
                MinuleZavody = minule,
                Kolobezky = kolobezky,
                CelkovaCastka = celkovaCastka,
                PocetStartu = pocetStartu,
                DostupneZavody = dostupneZavody
            };

            if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"];
            }

            return View(vm);
        }

        // ===========================================
        //       VYTVOŘENÍ ZÁVODNÍKA K ÚČTU
        // ===========================================
        [HttpGet]
        public IActionResult VytvorZavodnika()
        {
            var check = RequireLogin();
            if (check != null) return check;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VytvorZavodnika(string jmeno, string prijmeni, int vek, string pohlavi, string urovenZkusenosti)
        {
            var check = RequireLogin();
            if (check != null) return check;

            if (string.IsNullOrWhiteSpace(jmeno) ||
                string.IsNullOrWhiteSpace(prijmeni) ||
                string.IsNullOrWhiteSpace(pohlavi) ||
                string.IsNullOrWhiteSpace(urovenZkusenosti))
            {
                ViewBag.Error = "Vyplň všechna povinná pole.";
                return View();
            }

            var userId = CurrentUserId!.Value;
            var uzivatel = _context.Uzivatele.FirstOrDefault(u => u.IdUzivatel == userId);

            if (uzivatel == null)
            {
                return RedirectToAction("Login");
            }

            if (uzivatel.IdZavodnik.HasValue)
            {
                return RedirectToAction("Profil");
            }

            var zavodnik = new Zavodnik
            {
                Jmeno = jmeno,
                Prijmeni = prijmeni,
                Vek = vek,
                Pohlavi = pohlavi,
                UrovenZkusenosti = urovenZkusenosti
            };

            _context.Zavodnici.Add(zavodnik);
            _context.SaveChanges();

            uzivatel.IdZavodnik = zavodnik.IdZavodnik;
            _context.SaveChanges();

            TempData["Success"] = "Závodnický profil byl vytvořen.";
            return RedirectToAction("Profil");
        }

        // ===========================================
        //          PŘIHLÁŠENÍ NA ZÁVOD
        // ===========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PrihlasitNaZavod(int idZavod)
        {
            var check = RequireLogin();
            if (check != null) return check;

            var userId = CurrentUserId!.Value;
            var uzivatel = _context.Uzivatele.FirstOrDefault(u => u.IdUzivatel == userId);

            if (uzivatel == null || !uzivatel.IdZavodnik.HasValue)
            {
                TempData["Success"] = null;
                TempData["Error"] = "K účtu není přiřazen závodník.";
                return RedirectToAction("Profil");
            }

            int idZavodnik = uzivatel.IdZavodnik.Value;

            bool uzPrihlasen = _context.Ucasti
                .Any(u => u.IdZavod == idZavod && u.IdZavodnik == idZavodnik);

            if (!uzPrihlasen)
            {
                var ucast = new Ucast
                {
                    IdZavod = idZavod,
                    IdZavodnik = idZavodnik
                };

                _context.Ucasti.Add(ucast);
                _context.SaveChanges();

                TempData["Success"] = "Byl jsi úspěšně přihlášen na závod.";
            }

            return RedirectToAction("Profil");
        }
    }
}
