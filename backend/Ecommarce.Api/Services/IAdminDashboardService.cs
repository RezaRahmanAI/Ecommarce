using Ecommarce.Api.Models;

namespace Ecommarce.Api.Services;

public interface IAdminDashboardService
{
    DashboardStats GetDashboardStats();
    List<OrderItem> GetRecentOrders();
    List<PopularProduct> GetPopularProducts();
}
