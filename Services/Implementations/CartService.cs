using Marketeer.Data;
using Marketeer.Models;
using Marketeer.Services.Interfaces;

namespace Marketeer.Services.Implementations;

public class CartService : ICartService
{
    private readonly ApplicationDbContext _context;

    public CartService(ApplicationDbContext context)
    {
        _context = context;
    }

    public Cart GetCart() => _context.Cart;

    public void AddToCart(int productId, int quantity = 1)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == productId);
        if (product is null || quantity <= 0)
        {
            return;
        }

        var existing = _context.Cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is null)
        {
            _context.Cart.Items.Add(new CartItem
            {
                ProductId = productId,
                Product = product,
                Quantity = quantity
            });
        }
        else
        {
            existing.Quantity += quantity;
        }
    }

    public void RemoveFromCart(int productId)
    {
        var item = _context.Cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item is not null)
        {
            _context.Cart.Items.Remove(item);
        }
    }

    public void ClearCart() => _context.Cart.Items.Clear();
}
