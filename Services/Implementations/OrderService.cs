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
        var cart = _cartService.GetCart();
        if (!cart.Items.Any())
        {
            return null;
        }

        var order = new Order
        {
            UserId = cart.Items.First().UserId,
            CustomerName = customerName,
            ShippingAddress = shippingAddress,
            CreatedAtUtc = DateTime.UtcNow,
            Items = cart.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                UnitPrice = i.Product.Price,
                Quantity = i.Quantity
            }).ToList()
        };

        _context.Orders.Add(order);
        _context.SaveChanges();
        _cartService.ClearCart();
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
