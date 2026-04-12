using Marketeer.Models;

namespace Marketeer.Services.Interfaces;

public interface IOrderService
{
    Order? PlaceOrder(string customerName, string shippingAddress);
    Order? GetById(int id);
    IEnumerable<Order> GetAll();
    IEnumerable<Order> GetByUserId(string userId);
}
