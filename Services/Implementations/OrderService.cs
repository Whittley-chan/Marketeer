using Marketeer.Data;
using Marketeer.Models;
using Marketeer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Marketeer.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly ICartService _cartService;

    public OrderService(ApplicationDbContext context, ICartService cartService)
    {
        _context = context;
        _cartService = cartService;
    }

    public Order? PlaceOrder(string customerName, string shippingAddress)
    {
        var cart = _cartService.GetSelectedCart();
        if (!cart.Items.Any())
        {
            return null;
        }

        // Validate available stock before reserving.
        foreach (var cartItem in cart.Items)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == cartItem.ProductId);
            if (product is null || product.AvailableQuantity < cartItem.Quantity)
            {
                return null;
            }
        }

        var order = new Order
        {
            UserId = cart.Items.First().UserId,
            CustomerName = customerName,
            ShippingAddress = shippingAddress,
            DeliveryStatus = "Pending",
            CreatedAtUtc = DateTime.UtcNow,
            Items = cart.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                UnitPrice = i.Product.Price,
                Quantity = i.Quantity
            }).ToList()
        };

        foreach (var cartItem in cart.Items)
        {
            var product = _context.Products.First(p => p.Id == cartItem.ProductId);
            product.AvailableQuantity -= cartItem.Quantity;
            product.ReservedQuantity += cartItem.Quantity;
        }

        _context.Orders.Add(order);
        _context.SaveChanges();
        _cartService.ClearCart(selectedOnly: true);
        return order;
    }

    public Order? GetById(int id) => _context.Orders
        .Include(o => o.Items)
        .AsNoTracking()
        .FirstOrDefault(o => o.Id == id);

    public IEnumerable<Order> GetAll() => _context.Orders
        .Include(o => o.Items)
        .AsNoTracking()
        .OrderByDescending(o => o.CreatedAtUtc)
        .ToList();
}
