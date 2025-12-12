using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoZavodyWeb.Data;
using MotoZavodyWeb.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;
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
        // PROFIL (Upraveno pro načítání dat)
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Profil()
        {
            var check = RequireLogin();
            if (check != null) return check;

            var uzivatel = await _context.Uzivatele
                .Include(u => u.Zavodnik)
                .FirstAsync(u => u.IdUzivatel == CurrentUserId);

            // 1. Načtení koloběžek
            var kolobezky = new List<Kolobezka>();

            // 2. Příprava seznamů pro závody
            var nadchazejici = new List<UcastDetailView>();
            var minule = new List<UcastDetailView>();
            var dostupne = new List<ZavodDetailView>();

            decimal celkovaCastka = 0;
            int pocetStartu = 0;

            // Pokud má uživatel vytvořený profil závodníka
            if (uzivatel.Zavodnik != null)
            {
                int zavodnikId = uzivatel.Zavodnik.IdZavodnik;

                // A) Načíst koloběžky
                kolobezky = await _context.JezdiNa
                    .Include(j => j.Kolobezka)
                    .Where(j => j.IdZavodnik == zavodnikId)
                    .Select(j => j.Kolobezka!)
                    .ToListAsync();

                // B) Načíst moje účasti (z View V_UCASTI_DETAIL)
                var mojeUcasti = await _context.UcastiDetail
                    .Where(u => u.IdZavodnik == zavodnikId)
                    .OrderBy(u => u.DatumZavodu)
                    .ToListAsync();

                foreach (var u in mojeUcasti)
                {
                    if (u.DatumZavodu >= DateTime.Today)
                        nadchazejici.Add(u);
                    else
                        minule.Add(u);

                    celkovaCastka += u.Castka;
                    pocetStartu++;
                }

                // C) Načíst dostupné závody (kde ještě nejsem přihlášen)
                // Získáme ID závodů, kde už jsem
                var mojeZavodyIds = await _context.Ucasti
                    .Where(u => u.IdZavodnik == zavodnikId)
                    .Select(u => u.IdZavod)
                    .ToListAsync();

                // Vybereme budoucí závody, které nejsou v mém seznamu
                dostupne = await _context.ZavodyDetail
                    .Where(z => z.Datum >= DateTime.Today && !mojeZavodyIds.Contains(z.IdZavod))
                    .OrderBy(z => z.Datum)
                    .ToListAsync();
            }

            // 3. Sestavení ViewModelu
            var vm = new UzivatelProfilViewModel
            {
                Uzivatel = uzivatel,
                Zavodnik = uzivatel.Zavodnik,
                Kolobezky = kolobezky,

                NadchazejiciZavody = nadchazejici,
                MinuleZavody = minule,
                PocetStartu = pocetStartu,
                CelkovaCastka = celkovaCastka,
                DostupneZavody = dostupne,
            };

            return View(vm);
        }

        // =====================================================
        // PŘIHLÁŠENÍ NA ZÁVOD Z PROFILU (Nová metoda)
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrihlasitNaZavod(int idZavod)
        {
            var check = RequireLogin();
            if (check != null) return check;

            var uzivatel = await _context.Uzivatele
                .Include(u => u.Zavodnik)
                .FirstAsync(u => u.IdUzivatel == CurrentUserId);

            if (uzivatel.Zavodnik == null)
            {
                TempData["Error"] = "Nejprve si vytvořte závodnický profil.";
                return RedirectToAction("Profil");
            }

            var zavod = await _context.Zavody.FindAsync(idZavod);
            if (zavod == null) return NotFound();

            try
            {
                var pIdZavodnik = new OracleParameter("p_id_zavodnik", OracleDbType.Int32, uzivatel.Zavodnik.IdZavodnik, ParameterDirection.Input);
                var pIdZavod = new OracleParameter("p_id_zavod", OracleDbType.Int32, idZavod, ParameterDirection.Input);
                var pCastka = new OracleParameter("p_castka", OracleDbType.Decimal, zavod.Startovne, ParameterDirection.Input);
                var pTypPlatby = new OracleParameter("p_typ_platby", OracleDbType.Char, "H", ParameterDirection.Input);
                var pCisloKarty = new OracleParameter("p_cislo_karty", OracleDbType.Varchar2, DBNull.Value, ParameterDirection.Input);

                string sql = "BEGIN PR_PRIHLAS_ZAVODNIKA_DO_ZAVODU(:p_id_zavodnik, :p_id_zavod, :p_castka, :p_typ_platby, :p_cislo_karty); END;";

                await _context.Database.ExecuteSqlRawAsync(sql, pIdZavodnik, pIdZavod, pCastka, pTypPlatby, pCisloKarty);

                TempData["Success"] = "Byli jste úspěšně přihlášeni na závod.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Chyba při přihlašování: " + ex.Message;
            }

            return RedirectToAction("Profil");
        }

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

            var uzivatel = _context.Uzivatele.First(u => u.IdUzivatel == CurrentUserId);

            if (uzivatel.IdZavodnik.HasValue)
                return RedirectToAction("Profil");

            // předvyplnění jména z účtu
            var model = new Zavodnik
            {
                Jmeno = uzivatel.Jmeno,
                Prijmeni = uzivatel.Prijmeni
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VytvorZavodnika(Zavodnik model)
        {
            var check = RequireLogin();
            if (check != null) return check;

            if (!ModelState.IsValid)
            {
                // validace z DataAnnotations → zobrazí se ve view
                return View(model);
            }

            var uzivatel = _context.Uzivatele.First(u => u.IdUzivatel == CurrentUserId);

            if (uzivatel.IdZavodnik.HasValue)
                return RedirectToAction("Profil");

            _context.Zavodnici.Add(model);
            _context.SaveChanges();

            uzivatel.IdZavodnik = model.IdZavodnik;
            _context.SaveChanges();

            TempData["Success"] = "Závodnický profil byl vytvořen!";
            return RedirectToAction("Profil");
        }

        // =====================================================
        // SMAZAT ZÁVODNICKÝ PROFIL
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SmazatZavodnickyProfil()
        {
            var check = RequireLogin();
            if (check != null) return check;

            var uzivatel = await _context.Uzivatele
                .Include(u => u.Zavodnik)
                .FirstAsync(u => u.IdUzivatel == CurrentUserId);

            if (uzivatel.Zavodnik == null)
            {
                TempData["Error"] = "Závodnický profil neexistuje.";
                return RedirectToAction("Profil");
            }

            int zavodnikId = uzivatel.Zavodnik.IdZavodnik;

            var ucasti = _context.Ucasti.Where(u => u.IdZavodnik == zavodnikId);
            _context.Ucasti.RemoveRange(ucasti);

            var jezdiNa = _context.JezdiNa.Where(j => j.IdZavodnik == zavodnikId);
            _context.JezdiNa.RemoveRange(jezdiNa);

            uzivatel.IdZavodnik = null;

            _context.Zavodnici.Remove(uzivatel.Zavodnik);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Závodnický profil byl úspěšně smazán.";
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