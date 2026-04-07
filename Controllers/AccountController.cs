using Microsoft.AspNetCore.Mvc;

namespace Marketeer.Controllers;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError(string.Empty, "Email and password are required.");
            return View();
        }

        TempData["Message"] = "Authentication is disabled in this demo.";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public IActionResult Register(string fullName, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(fullName) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError(string.Empty, "All fields are required.");
            return View();
        }

        TempData["Message"] = "Registration is disabled in this demo.";
        return RedirectToAction("Index", "Home");
    }
}
