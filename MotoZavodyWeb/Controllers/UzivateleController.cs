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

        // =====================================================
        // HASH hesla
        // =====================================================
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }

        // =====================================================
        // REGISTRACE
        // =====================================================
        [HttpGet]
        public IActionResult Registrace()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registrace(string jmeno, string prijmeni, string email, string heslo)
        {
            if (string.IsNullOrWhiteSpace(jmeno) ||
                string.IsNullOrWhiteSpace(prijmeni) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(heslo))
            {
                ViewBag.Error = "Vyplň všechna pole.";
                return View();
            }

            if (_context.Uzivatele.Any(u => u.Email == email))
            {
                ViewBag.Error = "Uživatel s tímto e-mailem již existuje.";
                return View();
            }

            var uzivatel = new Uzivatel
            {
                Jmeno = jmeno,
                Prijmeni = prijmeni,
                Email = email,
                Heslo = HashPassword(heslo),
                Role = "USER",
                DatumVytvoreni = DateTime.Now
            };

            _context.Uzivatele.Add(uzivatel);
            _context.SaveChanges();

            TempData["Success"] = "Registrace proběhla úspěšně.";
            return RedirectToAction("Login");
        }

        // =====================================================
        // LOGIN
        // =====================================================
        [HttpGet]
        public IActionResult Login()
        {
            if (TempData["Success"] != null)
                ViewBag.Success = TempData["Success"];

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
            HttpContext.Session.SetString("UserFullName", uzivatel.Jmeno + " " + uzivatel.Prijmeni);


            uzivatel.DatumPoslednihoPrihlaseni = DateTime.Now;
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        // =====================================================
        // ODHLAŠENÍ
        // =====================================================
        public IActionResult Odhlasit()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // =====================================================
        // PROFIL
        // =====================================================
        [HttpGet]
        public IActionResult Profil()
        {
            var check = RequireLogin();
            if (check != null) return check;

            var uzivatel = _context.Uzivatele
                .Include(u => u.Zavodnik)
                .First(u => u.IdUzivatel == CurrentUserId);

            // ---------------------------
            // KOLOBĚŽKY
            // ---------------------------
            var kolobezky = new List<Kolobezka>();
            int zavodnikId = uzivatel.Zavodnik?.IdZavodnik ?? 0;

            if (uzivatel.Zavodnik != null)
            {
                kolobezky = _context.JezdiNa
                    .Include(j => j.Kolobezka)
                    .Where(j => j.IdZavodnik == zavodnikId)
                    .Select(j => j.Kolobezka!)
                    .ToList();
            }
            // ---------------------------
            // VIEWMODEL
            //----------------------------
            var vm = new UzivatelProfilViewModel
            {
                Uzivatel = uzivatel,
                Zavodnik = uzivatel.Zavodnik,
                Kolobezky = kolobezky,

                NadchazejiciZavody = new(),
                MinuleZavody = new(),
                PocetStartu = 0,
                CelkovaCastka = 0,
                DostupneZavody = new(),
            };

            return View(vm);
        }


          //    // ---------------------------
          //    // ZÁVODY UŽIVATELE
          //    // ---------------------------
          //    var nadchazejici = new List<ZavodUzivateleView>();
          //    var minule = new List<ZavodUzivateleView>();
          //    int pocetStartu = 0;
          //    decimal celkovaCastka = 0;
          //    if (uzivatel.Zavodnik != null)
          //    {
          //        int zavodnikId = uzivatel.Zavodnik.IdZavodnik;

          //        var prihlasky = _context.Prihlasky
          //            .Include(p => p.Zavod)
          //            .Where(p => p.IdZavodnik == zavodnikId)
          //            .ToList();

          //        foreach (var p in prihlasky)
          //        {
          //            var zaznam = new ZavodUzivateleView
          //            {
          //                NazevZavodu = p.Zavod.Nazev,
          //                DatumZavodu = p.Zavod.Datum,
          //                Castka = p.Castka,
          //                TypPlatby = p.TypPlatby
          //            };

          //            if (p.Zavod.Datum >= DateTime.Now)
          //                nadchazejici.Add(zaznam);
          //            else
          //                minule.Add(zaznam);

          //            pocetStartu++;
          //            celkovaCastka += p.Castka;
          //        }
          //    }

          //    // ---------------------------
          //    // DOSTUPNÉ ZÁVODY
          //    // ---------------------------
          //    var dostupne = _context.Zavody
          //        .Where(z => z.Datum >= DateTime.Now)
          //        .ToList();

          //    // ---------------------------
          //    // VIEWMODEL
          //    // ---------------------------
          //    var vm = new UzivatelProfilViewModel
          //    {
          //        Uzivatel = uzivatel,
          //        Zavodnik = uzivatel.Zavodnik,
          //        Kolobezky = kolobezky,
          //        NadchazejiciZavody = nadchazejici,
          //        MinuleZavody = minule,
          //        PocetStartu = pocetStartu,
          //        CelkovaCastka = celkovaCastka,
          //        DostupneZavody = dostupne
          //    };
          //    return View(vm);
          //}



        // =====================================================
        // PROFIL FOTO
        // =====================================================

        [HttpGet]
        public IActionResult NahratFoto()
        {
            var check = RequireLogin();
            if (check != null) return check;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NahratFoto(IFormFile foto)
        {
            var check = RequireLogin();
            if (check != null) return check;

            if (foto == null || foto.Length == 0)
            {
                ViewBag.Error = "Vyber prosím nějaký obrázek.";
                return View();
            }

            var user = _context.Uzivatele.First(u => u.IdUzivatel == CurrentUserId);

            using var ms = new MemoryStream();
            await foto.CopyToAsync(ms);
            user.ProfilFoto = ms.ToArray();

            await _context.SaveChangesAsync();

            TempData["Success"] = "Profilová fotka byla úspěšně nahrána.";
            return RedirectToAction("Profil");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OdstranitFoto()
        {
            var check = RequireLogin();
            if (check != null) return check;

            var user = _context.Uzivatele.First(u => u.IdUzivatel == CurrentUserId);

            user.ProfilFoto = null;
            _context.SaveChanges();

            TempData["Success"] = "Profilová fotka byla odstraněna.";
            return RedirectToAction("Profil");
        }



        // =====================================================
        // VYTVOŘENÍ ZÁVODNÍKA K ÚČTU
        // =====================================================
        [HttpGet]
        public IActionResult VytvorZavodnika()
        {
            var check = RequireLogin();
            if (check != null) return check;

            var userId = CurrentUserId!.Value;
            var uzivatel = _context.Uzivatele.First(u => u.IdUzivatel == userId);

            if (uzivatel.IdZavodnik.HasValue)
                return RedirectToAction("Profil");

            ViewBag.Jmeno = uzivatel.Jmeno;
            ViewBag.Prijmeni = uzivatel.Prijmeni;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VytvorZavodnika(string jmeno, string prijmeni, int vek, string pohlavi, string urovenZkusenosti)
        {
            var check = RequireLogin();
            if (check != null) return check;

            var userId = CurrentUserId!.Value;
            var uzivatel = _context.Uzivatele.First(u => u.IdUzivatel == userId);

            if (uzivatel.IdZavodnik.HasValue)
                return RedirectToAction("Profil");

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

            TempData["Success"] = "Závodnický profil byl vytvořen!";
            return RedirectToAction("Profil");
        }

        // =====================================================
        // PŘIDAT KOLOBĚŽKU
        // =====================================================
        [HttpGet]
        public IActionResult PridatKolobezku()
        {
            var check = RequireLogin();
            if (check != null) return check;

            var uzivatel = _context.Uzivatele
                .Include(u => u.Zavodnik)
                .First(u => u.IdUzivatel == CurrentUserId);

            if (uzivatel.Zavodnik == null)
            {
                TempData["Error"] = "Pro přidání koloběžky musíš mít vytvořený závodnický profil.";
                return RedirectToAction("Profil");
            }

            int zavodnikId = uzivatel.Zavodnik.IdZavodnik;

            var vsechny = _context.Kolobezky.ToList();
            var moje = _context.JezdiNa
                .Where(j => j.IdZavodnik == zavodnikId)
                .Select(j => j.IdKolobezka)
                .ToList();

            ViewBag.Kolobezky = vsechny
                .Where(k => !moje.Contains(k.IdKolobezka))
                .ToList();

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PridatKolobezku(int idKolobezka)
        {
            var uzivatel = _context.Uzivatele
                .Include(u => u.Zavodnik)
                .First(u => u.IdUzivatel == CurrentUserId);

            if (uzivatel.Zavodnik == null)
            {
                TempData["Error"] = "Pro přidání koloběžky musíš mít vytvořený závodnický profil.";
                return RedirectToAction("Profil");
            }

            int zavodnikId = uzivatel.Zavodnik.IdZavodnik;

            bool existuje = _context.JezdiNa
                .Count(j => j.IdZavodnik == zavodnikId && j.IdKolobezka == idKolobezka) > 0;


            if (!existuje)
            {
                _context.JezdiNa.Add(new JezdiNa
                {
                    IdZavodnik = zavodnikId,
                    IdKolobezka = idKolobezka
                });

                _context.SaveChanges();
                TempData["Success"] = "Koloběžka byla přidána.";
            }

            return RedirectToAction("Profil");
        }

        // =====================================================
        // ODEBRAT KOLOBĚŽKU
        // =====================================================
        public IActionResult OdebratKolobezku(int idKolobezka)
        {
            var uzivatel = _context.Uzivatele
                .Include(u => u.Zavodnik)
                .First(u => u.IdUzivatel == CurrentUserId);

            if (uzivatel.Zavodnik == null)
                return RedirectToAction("VytvorZavodnika");

            int zavodnikId = uzivatel.Zavodnik.IdZavodnik;

            var zaznam = _context.JezdiNa
                .FirstOrDefault(j => j.IdZavodnik == zavodnikId && j.IdKolobezka == idKolobezka);

            if (zaznam != null)
            {
                _context.JezdiNa.Remove(zaznam);
                _context.SaveChanges();
                TempData["Success"] = "Koloběžka byla odebrána.";
            }

            return RedirectToAction("Profil");
        }


    }
}



