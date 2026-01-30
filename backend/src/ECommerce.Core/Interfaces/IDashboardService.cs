using ECommerce.Core.DTOs;

namespace ECommerce.Core.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
    Task<List<RecentOrderDto>> GetRecentOrdersAsync();
    Task<List<PopularProductDto>> GetPopularProductsAsync();
}
