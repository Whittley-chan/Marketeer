using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Marketeer.Models;

namespace Marketeer.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AccountController(
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError(string.Empty, "Email and password are required.");
            return View();
        }

        var result = await _signInManager.PasswordSignInAsync(email, password, true, false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Email or password is invalid.");
            return View();
        }

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string fullName, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(fullName) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError(string.Empty, "All fields are required.");
            return View();
        }

        var existing = await _userManager.FindByEmailAsync(email);
        if (existing is not null)
        {
            ModelState.AddModelError(string.Empty, "Email is already registered.");
            return View();
        }

        var user = new User
        {
            FullName = fullName,
            UserName = email,
            Email = email
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }

        await _signInManager.SignInAsync(user, isPersistent: true);
        TempData["Message"] = $"Welcome, {fullName}!";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        return View(user);
    }

    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> EditProfile(string fullName, string email, string phoneNumber, DateTime dateOfBirth)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email))
        {
            ModelState.AddModelError(string.Empty, "Full name and email are required.");
            return View(user);
        }

        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null && existingUser.Id != user.Id)
        {
            ModelState.AddModelError(string.Empty, "Email is already in use.");
            return View(user);
        }

        user.FullName = fullName;
        user.Email = email;
        user.UserName = email;
        user.PhoneNumber = phoneNumber;
        user.DateOfBirth = dateOfBirth;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(user);
        }

        TempData["Message"] = "Profile updated successfully!";
        return RedirectToAction("Profile");
    }
}
