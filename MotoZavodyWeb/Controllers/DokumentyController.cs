using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MotoZavodyWeb.Data;
using MotoZavodyWeb.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace MotoZavodyWeb.Controllers
{
    public class DokumentyController : Controller
    {
        private readonly ZavodyContext _context;

        public DokumentyController(ZavodyContext context)
        {
            _context = context;
        }

        // GET: /Dokumenty
        public async Task<IActionResult> Index()
        {
            var docs = await _context.DokumentyZavodniku
                .Include(d => d.Zavodnik)
                .OrderBy(d => d.Zavodnik!.Prijmeni)
                .ThenBy(d => d.Zavodnik!.Jmeno)
                .ThenByDescending(d => d.DatumNahrani)
                .ToListAsync();

            return View(docs);
        }

        // GET: /Dokumenty/Create
        public IActionResult Create()
        {
            var model = new DokumentCreateViewModel
            {
                Zavodnici = _context.Zavodnici
                    .OrderBy(z => z.Prijmeni)
                    .ThenBy(z => z.Jmeno)
                    .Select(z => new SelectListItem
                    {
                        Value = z.IdZavodnik.ToString(),
                        Text = $"{z.Jmeno} {z.Prijmeni}"
                    })
                    .ToList()
            };

            return View(model);
        }

        // POST: /Dokumenty/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DokumentCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Zavodnici = _context.Zavodnici
                    .OrderBy(z => z.Prijmeni)
                    .ThenBy(z => z.Jmeno)
                    .Select(z => new SelectListItem
                    {
                        Value = z.IdZavodnik.ToString(),
                        Text = $"{z.Jmeno} {z.Prijmeni}"
                    })
                    .ToList();

                return View(model);
            }

            byte[] obsah;
            using (var ms = new MemoryStream())
            {
                await model.Soubor.CopyToAsync(ms);
                obsah = ms.ToArray();
            }

            var fileName = Path.GetFileNameWithoutExtension(model.Soubor.FileName);
            var ext = Path.GetExtension(model.Soubor.FileName).TrimStart('.');
            var contentType = model.Soubor.ContentType;

            var pIdZavodnik = new OracleParameter("p_id_zavodnik", OracleDbType.Int32, model.IdZavodnik, ParameterDirection.Input);
            var pNazev = new OracleParameter("p_nazev", OracleDbType.Varchar2, fileName, ParameterDirection.Input);
            var pTyp = new OracleParameter("p_typ", OracleDbType.Varchar2, contentType, ParameterDirection.Input);
            var pPripona = new OracleParameter("p_pripona", OracleDbType.Varchar2, ext, ParameterDirection.Input);
            var pObsah = new OracleParameter("p_obsah", OracleDbType.Blob, obsah, ParameterDirection.Input);

            var sql = "BEGIN PR_NAHRAJ_DOKUMENT_ZAVODNIKA(:p_id_zavodnik, :p_nazev, :p_typ, :p_pripona, :p_obsah); END;";

            await _context.Database.ExecuteSqlRawAsync(sql,
                pIdZavodnik, pNazev, pTyp, pPripona, pObsah);

            return RedirectToAction(nameof(Index));
        }

        // GET: /Dokumenty/Download/5
        public async Task<IActionResult> Download(int id)
        {
            var doc = await _context.DokumentyZavodniku
                .FirstOrDefaultAsync(d => d.IdDokument == id);

            if (doc == null)
                return NotFound();

            var fileName = $"{doc.NazevSouboru}.{doc.PriponaSouboru}";
            return File(doc.Obsah, doc.TypSouboru, fileName);
        }
    }
}
