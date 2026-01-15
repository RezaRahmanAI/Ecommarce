using Ecommarce.Api.Models;
using Ecommarce.Api.Repositories;

namespace Ecommarce.Api.Services;

public sealed class AdminDashboardService : IAdminDashboardService
{
    private readonly IAdminRepository _repository;

    public AdminDashboardService(IAdminRepository repository)
    {
        _repository = repository;
    }

    public DashboardStats GetDashboardStats() => _repository.GetDashboardStats();

    public List<OrderItem> GetRecentOrders() => _repository.GetRecentOrders();

    public List<PopularProduct> GetPopularProducts() => _repository.GetPopularProducts();
}
