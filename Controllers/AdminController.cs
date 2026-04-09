using Marketeer.Data;
using Marketeer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Marketeer.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index() => RedirectToAction(nameof(Products));

    [HttpGet]
    public async Task<IActionResult> Users()
    {
        var users = await _userManager.Users.OrderBy(u => u.Email).ToListAsync();
        var model = new List<AdminUserViewModel>();

        foreach (var user in users)
        {
            model.Add(new AdminUserViewModel
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                IsAdmin = await _userManager.IsInRoleAsync(user, "Admin")
            });
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleAdminRole(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return RedirectToAction(nameof(Users));
        }

        if (string.Equals(User.Identity?.Name, user.Email, StringComparison.OrdinalIgnoreCase))
        {
            TempData["Message"] = "You cannot change your own admin role.";
            return RedirectToAction(nameof(Users));
        }

        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            await _userManager.RemoveFromRoleAsync(user, "Admin");
        }
        else
        {
            await _userManager.AddToRoleAsync(user, "Admin");
        }

        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return RedirectToAction(nameof(Users));
        }

        if (string.Equals(User.Identity?.Name, user.Email, StringComparison.OrdinalIgnoreCase))
        {
            TempData["Message"] = "You cannot delete your own account.";
            return RedirectToAction(nameof(Users));
        }

        await _userManager.DeleteAsync(user);
        return RedirectToAction(nameof(Users));
    }

    [HttpGet]
    public IActionResult Products()
    {
        var products = _context.Products
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToList();

        return View(products);
    }

    [HttpGet]
    public IActionResult CreateProduct()
    {
        ViewBag.Categories = _context.Categories.OrderBy(c => c.Name).ToList();
        return View(new AdminProductViewModel());
    }

    [HttpPost]
    public IActionResult CreateProduct(AdminProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = _context.Categories.OrderBy(c => c.Name).ToList();
            return View(model);
        }

        var product = new Product
        {
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            ImageUrl = model.ImageUrl,
            AvailableQuantity = model.AvailableQuantity,
            ReservedQuantity = model.ReservedQuantity,
            CategoryId = model.CategoryId
        };

        _context.Products.Add(product);
        _context.SaveChanges();
        return RedirectToAction(nameof(Products));
    }

    [HttpGet]
    public IActionResult EditProduct(int id)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == id);
        if (product is null)
        {
            return RedirectToAction(nameof(Products));
        }

        ViewBag.Categories = _context.Categories.OrderBy(c => c.Name).ToList();
        return View(new AdminProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            AvailableQuantity = product.AvailableQuantity,
            ReservedQuantity = product.ReservedQuantity,
            CategoryId = product.CategoryId
        });
    }

    [HttpPost]
    public IActionResult EditProduct(AdminProductViewModel model)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == model.Id);
        if (product is null)
        {
            return RedirectToAction(nameof(Products));
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = _context.Categories.OrderBy(c => c.Name).ToList();
            return View(model);
        }

        product.Name = model.Name;
        product.Description = model.Description;
        product.Price = model.Price;
        product.ImageUrl = model.ImageUrl;
        product.AvailableQuantity = model.AvailableQuantity;
        product.ReservedQuantity = model.ReservedQuantity;
        product.CategoryId = model.CategoryId;

        _context.SaveChanges();
        return RedirectToAction(nameof(Products));
    }

    [HttpPost]
    public IActionResult DeleteProduct(int id)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == id);
        if (product is null)
        {
            return RedirectToAction(nameof(Products));
        }

        _context.Products.Remove(product);
        _context.SaveChanges();
        return RedirectToAction(nameof(Products));
    }
}
