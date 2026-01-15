using Ecommarce.Api.Models;

namespace Ecommarce.Api.Services;

public interface IAdminOrderService
{
    List<Order> FilterOrders(string? searchTerm, string status, string dateRange);
    Order CreateOrder(OrderCreatePayload payload);
    Order? UpdateOrderStatus(int id, OrderStatus status);
    bool DeleteOrder(int id);
}
