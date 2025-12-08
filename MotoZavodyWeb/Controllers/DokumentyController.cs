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
            var role = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetInt32("UserId");

            IQueryable<DokumentZavodnika> query = _context.DokumentyZavodniku
                .Include(d => d.Zavodnik)
                .OrderBy(d => d.Zavodnik!.Prijmeni)
                .ThenBy(d => d.Zavodnik!.Jmeno)
                .ThenByDescending(d => d.DatumNahrani);

            // Pokud je user, zobrazí se jen dokumenty jeho závodníka
            if (role != "ADMIN")
            {
                var user = await _context.Uzivatele
                    .Include(u => u.Zavodnik)
                    .FirstOrDefaultAsync(u => u.IdUzivatel == userId);

                if (user?.Zavodnik != null)
                {
                    query = query.Where(d => d.IdZavodnik == user.Zavodnik.IdZavodnik);
                }
                else
                {
                    // uživatel bez závodníka nic neuvidí
                    query = query.Where(d => false);
                }
            }

            return View(await query.ToListAsync());

            //return View(docs);
        }

        // GET: /Dokumenty/Create
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "ADMIN")
                return Unauthorized();

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
            var contentType = ext.ToLower();

            var pIdZavodnik = new OracleParameter("1", OracleDbType.Int32, model.IdZavodnik, ParameterDirection.Input);
            var pNazev = new OracleParameter("2", OracleDbType.Varchar2, fileName, ParameterDirection.Input);
            var pTyp = new OracleParameter("3", OracleDbType.Varchar2, contentType, ParameterDirection.Input);
            var pPripona = new OracleParameter("4", OracleDbType.Varchar2, ext, ParameterDirection.Input);
            var pObsah = new OracleParameter("5", OracleDbType.Blob, obsah, ParameterDirection.Input);
            var pViditelne = new OracleParameter("6", OracleDbType.Int16, model.ViditelneVsem ? 1 : 0, ParameterDirection.Input);


            string sql = "BEGIN PR_NAHRAJ_DOKUMENT_ZAVODNIKA(:1, :2, :3, :4, :5, :6); END;";

            await _context.Database.ExecuteSqlRawAsync(sql, pIdZavodnik, pNazev, pTyp, pPripona, pObsah, pViditelne);


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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "ADMIN")
                return Unauthorized();

            var doc = await _context.DokumentyZavodniku
                .FirstOrDefaultAsync(d => d.IdDokument == id);

            if (doc == null)
                return NotFound();

            _context.DokumentyZavodniku.Remove(doc);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}
