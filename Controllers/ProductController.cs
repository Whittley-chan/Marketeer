using Marketeer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Marketeer.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    public IActionResult Index(string? search, int? categoryId)
    {
        ViewBag.Search = search;
        ViewBag.CategoryId = categoryId;
        ViewBag.Categories = _productService.GetCategories();

        var products = _productService.GetAll(search, categoryId);
        return View(products);
    }

    public IActionResult Details(int id)
    {
        var product = _productService.GetById(id);
        if (product is null)
        {
            return NotFound();
        }

        return View(product);
    }
}
