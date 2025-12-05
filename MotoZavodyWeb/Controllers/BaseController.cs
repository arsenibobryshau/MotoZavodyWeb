using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MotoZavodyWeb.Controllers
{
    public abstract class BaseController : Controller
    {
        protected int? CurrentUserId =>
            HttpContext.Session.GetInt32("UserId");

        protected string? CurrentUserRole =>
            HttpContext.Session.GetString("UserRole");

        protected bool IsAdmin =>
            string.Equals(CurrentUserRole, "ADMIN", StringComparison.OrdinalIgnoreCase);

        protected IActionResult? RequireLogin()
        {
            if (!CurrentUserId.HasValue)
            {
                return RedirectToAction("Login", "Uzivatele");
            }

            return null;
        }

        protected IActionResult? RequireAdmin()
        {
            var loginCheck = RequireLogin();
            if (loginCheck != null) return loginCheck;

            if (!IsAdmin)
            {
                return Forbid();
            }

            return null;
        }
    }
}
