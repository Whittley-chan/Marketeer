using Marketeer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Marketeer.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productService;

    public HomeController(IProductService productService)
    {
        _productService = productService;
    }

    public IActionResult Index()
    {
        var products = _productService.GetAll().Take(4);
        return View(products);
    }
}
