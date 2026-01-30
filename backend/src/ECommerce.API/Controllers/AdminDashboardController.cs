using ECommerce.Core.DTOs;
using ECommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/admin/dashboard")]
//[Authorize(Roles = "admin")] // TODO: Enable this once roles are fully working and seeded
public class AdminDashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public AdminDashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStatsDto>> GetStats()
    {
        var stats = await _dashboardService.GetDashboardStatsAsync();
        return Ok(stats);
    }

    [HttpGet("orders/recent")]
    public async Task<ActionResult<List<RecentOrderDto>>> GetRecentOrders()
    {
        var orders = await _dashboardService.GetRecentOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("products/popular")]
    public async Task<ActionResult<List<PopularProductDto>>> GetPopularProducts()
    {
        var products = await _dashboardService.GetPopularProductsAsync();
        return Ok(products);
    }
}
