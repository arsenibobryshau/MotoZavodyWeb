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

        // pomocná metoda pro naplnění comboboxů
        private PrihlaskaCreateViewModel BuildCreateViewModel()
        {
            var model = new PrihlaskaCreateViewModel();

            model.Zavodnici = _context.Zavodnici
                .OrderBy(z => z.Prijmeni)
                .ThenBy(z => z.Jmeno)
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

        // GET: /Ucasti/Create
        public IActionResult Create()
        {
            var model = BuildCreateViewModel();
            return View(model);
        }

        // POST: /Ucasti/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PrihlaskaCreateViewModel model)
        {
            // Když neprojde validace modelu → vrátíme zpět view s chybou
            if (!ModelState.IsValid)
            {
                var vm = BuildCreateViewModel();
                vm.IdZavodnik = model.IdZavodnik;
                vm.IdZavod = model.IdZavod;
                vm.Castka = model.Castka;
                vm.TypPlatby = model.TypPlatby;
                vm.CisloKarty = model.CisloKarty;

                return View(vm);
            }

            try
            {
                var pIdZavodnik = new OracleParameter("p_id_zavodnik", OracleDbType.Int32, model.IdZavodnik, ParameterDirection.Input);
                var pIdZavod = new OracleParameter("p_id_zavod", OracleDbType.Int32, model.IdZavod, ParameterDirection.Input);
                var pCastka = new OracleParameter("p_castka", OracleDbType.Decimal, model.Castka, ParameterDirection.Input);
                var pTypPlatby = new OracleParameter("p_typ_platby", OracleDbType.Char, model.TypPlatby, ParameterDirection.Input);

                var pCisloKarty = new OracleParameter("p_cislo_karty", OracleDbType.Varchar2)
                {
                    Direction = ParameterDirection.Input,
                    Value = string.IsNullOrWhiteSpace(model.CisloKarty) ? (object)DBNull.Value : model.CisloKarty
                };

                var sql = "BEGIN PR_PRIHLAS_ZAVODNIKA_DO_ZAVODU(:p_id_zavodnik, :p_id_zavod, :p_castka, :p_typ_platby, :p_cislo_karty); END;";

                await _context.Database.ExecuteSqlRawAsync(sql,
                    pIdZavodnik, pIdZavod, pCastka, pTypPlatby, pCisloKarty);

                return RedirectToAction(nameof(Index));
            }
            catch (OracleException ex)
            {
                if (ex.Number == 1)
                {
                    ModelState.AddModelError("", "⚠ Tento závodník je již na tento závod přihlášen.");

                    var vm = BuildCreateViewModel();
                    vm.IdZavodnik = model.IdZavodnik;
                    vm.IdZavod = model.IdZavod;
                    vm.Castka = model.Castka;
                    vm.TypPlatby = model.TypPlatby;
                    vm.CisloKarty = model.CisloKarty;

                    return View(vm);
                }

                throw;
            }
        }


    }
}
