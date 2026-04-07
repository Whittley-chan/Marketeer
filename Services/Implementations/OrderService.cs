using Marketeer.Data;
using Marketeer.Models;
using Marketeer.Services.Interfaces;

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
            Id = _context.Orders.Count == 0 ? 1 : _context.Orders.Max(o => o.Id) + 1,
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
        _cartService.ClearCart();
        return order;
    }

    public Order? GetById(int id) => _context.Orders.FirstOrDefault(o => o.Id == id);

    public IEnumerable<Order> GetAll() => _context.Orders.OrderByDescending(o => o.CreatedAtUtc);
}
