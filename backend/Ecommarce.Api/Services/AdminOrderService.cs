using Ecommarce.Api.Models;
using Ecommarce.Api.Repositories;

namespace Ecommarce.Api.Services;

public sealed class AdminOrderService : IAdminOrderService
{
    private readonly IAdminRepository _repository;

    public AdminOrderService(IAdminRepository repository)
    {
        _repository = repository;
    }

    public List<Order> FilterOrders(string? searchTerm, string status, string dateRange)
        => _repository.FilterOrders(searchTerm, status, dateRange);

    public Order CreateOrder(OrderCreatePayload payload) => _repository.CreateOrder(payload);

    public Order? UpdateOrderStatus(int id, OrderStatus status) => _repository.UpdateOrderStatus(id, status);

    public bool DeleteOrder(int id) => _repository.DeleteOrder(id);
}
