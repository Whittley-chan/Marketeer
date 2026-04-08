using Marketeer.Data;
using Marketeer.Models;
using Marketeer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Marketeer.Services.Implementations;

public class CartService : ICartService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public Cart GetCart()
    {
        var userId = ResolveUserId();
        var items = _context.CartItems
            .Include(i => i.Product)
            .Where(i => i.UserId == userId)
            .OrderBy(i => i.Id)
            .ToList();

        return new Cart { Items = items };
    }

    public void AddToCart(int productId, int quantity = 1)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == productId);
        if (product is null || quantity <= 0)
        {
            return;
        }

        var userId = ResolveUserId();
        var existing = _context.CartItems
            .FirstOrDefault(i => i.UserId == userId && i.ProductId == productId);
        if (existing is null)
        {
            _context.CartItems.Add(new CartItem
            {
                UserId = userId,
                ProductId = productId,
                Product = product,
                Quantity = quantity
            });
        }
        else
        {
            existing.Quantity += quantity;
        }

        _context.SaveChanges();
    }

    public void RemoveFromCart(int productId)
    {
        var userId = ResolveUserId();
        var item = _context.CartItems
            .FirstOrDefault(i => i.UserId == userId && i.ProductId == productId);
        if (item is not null)
        {
            _context.CartItems.Remove(item);
            _context.SaveChanges();
        }
    }

    public void ClearCart()
    {
        var userId = ResolveUserId();
        var items = _context.CartItems.Where(i => i.UserId == userId).ToList();
        if (items.Count == 0)
        {
            return;
        }

        _context.CartItems.RemoveRange(items);
        _context.SaveChanges();
    }

    private string ResolveUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated == true && !string.IsNullOrWhiteSpace(user.Identity.Name))
        {
            return user.Identity.Name;
        }

        return "guest";
    }
}
