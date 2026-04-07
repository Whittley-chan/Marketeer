using Marketeer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Marketeer.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    public IActionResult Index()
    {
        var cart = _cartService.GetCart();
        return View(cart);
    }

    [HttpPost]
    public IActionResult Add(int productId, int quantity = 1)
    {
        _cartService.AddToCart(productId, quantity);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Remove(int productId)
    {
        _cartService.RemoveFromCart(productId);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Clear()
    {
        _cartService.ClearCart();
        return RedirectToAction(nameof(Index));
    }
}
