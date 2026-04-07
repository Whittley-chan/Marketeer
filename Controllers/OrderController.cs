using Marketeer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Marketeer.Controllers;

public class OrderController : Controller
{
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;

    public OrderController(ICartService cartService, IOrderService orderService)
    {
        _cartService = cartService;
        _orderService = orderService;
    }

    [HttpGet]
    public IActionResult Checkout()
    {
        var cart = _cartService.GetCart();
        if (!cart.Items.Any())
        {
            return RedirectToAction("Index", "Cart");
        }

        return View(cart);
    }

    [HttpPost]
    public IActionResult Checkout(string customerName, string shippingAddress)
    {
        if (string.IsNullOrWhiteSpace(customerName) || string.IsNullOrWhiteSpace(shippingAddress))
        {
            ModelState.AddModelError(string.Empty, "Name and shipping address are required.");
            return View(_cartService.GetCart());
        }

        var order = _orderService.PlaceOrder(customerName, shippingAddress);
        if (order is null)
        {
            return RedirectToAction("Index", "Cart");
        }

        return RedirectToAction(nameof(Success), new { id = order.Id });
    }

    [HttpGet]
    public IActionResult Success(int id)
    {
        var order = _orderService.GetById(id);
        if (order is null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(order);
    }
}
