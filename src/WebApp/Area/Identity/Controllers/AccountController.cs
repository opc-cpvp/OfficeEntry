using Microsoft.AspNetCore.Mvc;

namespace OfficeEntry.WebApp.Area.Identity.Controllers;

[Route("{controller}/{action}")]
public class AccountController : Controller
{
    public IActionResult Login(string returnUrl = "/")
    {
        return RedirectToAction("Login", "External", new { returnUrl });
    }
}
