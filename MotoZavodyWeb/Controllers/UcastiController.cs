using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MotoZavodyWeb.Data;
using MotoZavodyWeb.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace MotoZavodyWeb.Controllers
{
    public class UcastiController : Controller
    {
        private readonly ZavodyContext _context;

        public UcastiController(ZavodyContext context)
        {
            _context = context;
        }

        // GET: /Ucasti
        public async Task<IActionResult> Index()
        {
            var ucasti = await _context.UcastiDetail
                .OrderBy(u => u.DatumZavodu)
                .ThenBy(u => u.Prijmeni)
                .ThenBy(u => u.Jmeno)
                .ToListAsync();

            return View(ucasti);
        }

        // ==========================
        // SHARED BUILDER
        // ==========================
        private PrihlaskaCreateViewModel BuildCreateViewModel()
        {
            var model = new PrihlaskaCreateViewModel();

            model.Zavodnici = _context.Zavodnici
                .OrderBy(z => z.Prijmeni)
                .Select(z => new SelectListItem
                {
                    Value = z.IdZavodnik.ToString(),
                    Text = $"{z.Jmeno} {z.Prijmeni}"
                })
                .ToList();

            model.Zavody = _context.Zavody
                .OrderBy(z => z.Datum)
                .Select(z => new SelectListItem
                {
                    Value = z.IdZavod.ToString(),
                    Text = $"{z.Nazev} ({z.Datum:dd.MM.yyyy})"
                })
                .ToList();

            model.StartovneDict = _context.Zavody
                .Select(z => new { z.IdZavod, z.Startovne })
                .ToDictionary(x => x.IdZavod, x => x.Startovne);

            return model;
        }

        // =====================================
        // ADMIN — PRIHLAŠOVÁNÍ LIBOVOLNÉHO ZÁVODNÍKA
        // =====================================
        public IActionResult Create()
        {
            return View(BuildCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PrihlaskaCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var vm = BuildCreateViewModel();
                vm.IdZavodnik = model.IdZavodnik;
                vm.IdZavod = model.IdZavod;
                vm.Castka = model.Castka;
                vm.TypPlatby = model.TypPlatby;
                vm.CisloKarty = model.CisloKarty;

                // Pokud je uživatel přihlášen, obnovíme jeho jméno (pro readonly pole)
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId != null)
                {
                    var user = _context.Uzivatele.Include(u => u.Zavodnik).FirstOrDefault(u => u.IdUzivatel == userId);
                    if (user?.Zavodnik != null)
                    {
                        vm.JmenoZavodnika = $"{user.Zavodnik.Jmeno} {user.Zavodnik.Prijmeni}";
                    }
                }

                return View(vm);
            }

            return await ProvedPrihlaseni(model);
        }

        // =====================================
        // UŽIVATEL — PŘIHLÁŠENÍ SÁM SEBE
        // =====================================
        public async Task<IActionResult> PrihlasitSe(int? id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Uzivatele");

            var user = _context.Uzivatele
                .Include(u => u.Zavodnik)
                .FirstOrDefault(u => u.IdUzivatel == userId);

            if (user == null)
                return RedirectToAction("Login", "Uzivatele");

            if (user.Zavodnik == null)
            {
                TempData["Error"] = "Musíte si nejprve vytvořit závodnický profil.";
                return RedirectToAction("Profil", "Uzivatele");
            }

            var vm = BuildCreateViewModel();
            vm.IdZavodnik = user.Zavodnik.IdZavodnik;
            vm.JmenoZavodnika = $"{user.Zavodnik.Jmeno} {user.Zavodnik.Prijmeni}";

            // POKUD PŘIŠLO ID ZÁVODU, PŘEDVYPLNÍME HO A CENU
            if (id.HasValue)
            {
                var zavod = await _context.Zavody.FindAsync(id.Value);
                if (zavod != null)
                {
                    vm.IdZavod = zavod.IdZavod;
                    vm.Castka = zavod.Startovne;
                }
            }
            // Pokud ID nepřišlo, vm.IdZavod zůstane 0 a ve View se zobrazí Select

            return View("Create", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrihlasitSe(PrihlaskaCreateViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Uzivatele");

            var user = _context.Uzivatele
                .Include(u => u.Zavodnik)
                .FirstOrDefault(u => u.IdUzivatel == userId);

            if (user?.Zavodnik == null)
                return RedirectToAction("Profil", "Uzivatele");

            model.IdZavodnik = user.Zavodnik.IdZavodnik;

            return await ProvedPrihlaseni(model);
        }

        // =====================================
        // SPOLEČNÉ PROVEDENÍ ZÁPISU DO DB
        // =====================================
        private async Task<IActionResult> ProvedPrihlaseni(PrihlaskaCreateViewModel model)
        {
            try
            {
                var pIdZavodnik = new OracleParameter("p_id_zavodnik", OracleDbType.Int32, model.IdZavodnik, ParameterDirection.Input);
                var pIdZavod = new OracleParameter("p_id_zavod", OracleDbType.Int32, model.IdZavod, ParameterDirection.Input);
                var pCastka = new OracleParameter("p_castka", OracleDbType.Decimal, model.Castka, ParameterDirection.Input);
                var pTypPlatby = new OracleParameter("p_typ_platby", OracleDbType.Char, model.TypPlatby, ParameterDirection.Input);

                var pCisloKarty = new OracleParameter("p_cislo_karty", OracleDbType.Varchar2)
                {
                    Direction = ParameterDirection.Input,
                    Value = string.IsNullOrWhiteSpace(model.CisloKarty)
                            ? (object)DBNull.Value
                            : model.CisloKarty
                };

                string sql = "BEGIN PR_PRIHLAS_ZAVODNIKA_DO_ZAVODU(:p_id_zavodnik, :p_id_zavod, :p_castka, :p_typ_platby, :p_cislo_karty); END;";

                await _context.Database.ExecuteSqlRawAsync(sql,
                    pIdZavodnik, pIdZavod, pCastka, pTypPlatby, pCisloKarty);

                return RedirectToAction(nameof(Index));
            }
            catch (OracleException ex)
            {
                // chyba 1 = duplicitní klíč (závodník je již přihlášen)
                if (ex.Number == 1)
                {
                    ModelState.AddModelError("", "⚠ Na tento závod jste již přihlášeni.");

                    var vm = BuildCreateViewModel();
                    vm.IdZavod = model.IdZavod;
                    vm.Castka = model.Castka;
                    vm.TypPlatby = model.TypPlatby;
                    vm.CisloKarty = model.CisloKarty;

                    // Pokud je to user, znovu načteme jméno
                    var userId = HttpContext.Session.GetInt32("UserId");
                    if (userId != null)
                    {
                        var user = _context.Uzivatele.Include(u => u.Zavodnik).FirstOrDefault(u => u.IdUzivatel == userId);
                        if (user?.Zavodnik != null)
                        {
                            vm.JmenoZavodnika = $"{user.Zavodnik.Jmeno} {user.Zavodnik.Prijmeni}";
                        }
                    }

                    return View("Create", vm);
                }
                throw;
            }
        }

        // =====================================
        // DELETE
        // =====================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int idZavodnik, int idZavod)
        {
            if (HttpContext.Session.GetString("UserRole") != "ADMIN")
                return Unauthorized();

            var ucast = _context.Ucasti
                .FirstOrDefault(u => u.IdZavodnik == idZavodnik && u.IdZavod == idZavod);

            if (ucast == null)
                return NotFound();

            _context.Ucasti.Remove(ucast);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}