using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoZavodyWeb.Data;
using MotoZavodyWeb.Models;
using Oracle.ManagedDataAccess.Client;
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

        // GET: /Zavodnici
        public async Task<IActionResult> Index()
        {
            var zavodnici = await _context.Zavodnici
                .OrderBy(z => z.Prijmeni)
                .ThenBy(z => z.Jmeno)
                .ToListAsync();

            return View(zavodnici);
        }

        // GET: /Zavodnici/Create
        public IActionResult Create()
        {
            var model = new ZavodnikCreateViewModel();
            return View(model);
        }

        // POST: /Zavodnici/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ZavodnikCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // voláme uloženou proceduru PR_REGISTRUJ_ZAVODNIKA
            var pJmeno = new OracleParameter("p_jmeno", OracleDbType.Varchar2, model.Jmeno, ParameterDirection.Input);
            var pPrijmeni = new OracleParameter("p_prijmeni", OracleDbType.Varchar2, model.Prijmeni, ParameterDirection.Input);
            var pVek = new OracleParameter("p_vek", OracleDbType.Int32, model.Vek, ParameterDirection.Input);
            var pPohlavi = new OracleParameter("p_pohlavi", OracleDbType.Char, model.Pohlavi, ParameterDirection.Input);
            var pUroven = new OracleParameter("p_uroven_zkusenosti", OracleDbType.Char, model.UrovenZkusenosti, ParameterDirection.Input);
            var pIdOut = new OracleParameter("p_id_zavodnik_out", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };

            var sql = "BEGIN PR_REGISTRUJ_ZAVODNIKA(:p_jmeno, :p_prijmeni, :p_vek, :p_pohlavi, :p_uroven_zkusenosti, :p_id_zavodnik_out); END;";

            await _context.Database.ExecuteSqlRawAsync(sql,
                pJmeno, pPrijmeni, pVek, pPohlavi, pUroven, pIdOut);

            // pIdOut.Value obsahuje nové ID závodníka – mùžeme ho pozdìji použít
            return RedirectToAction(nameof(Index));
        }
    }
}
