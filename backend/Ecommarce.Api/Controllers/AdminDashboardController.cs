using Ecommarce.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommarce.Api.Controllers;

[ApiController]
[Route("api/admin/dashboard")]
public sealed class AdminDashboardController : ControllerBase
{
    private readonly IAdminDashboardService _dashboardService;

    public AdminDashboardController(IAdminDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("stats")]
    public IActionResult GetStats()
    {
        return Ok(_dashboardService.GetDashboardStats());
    }

    [HttpGet("orders/recent")]
    public IActionResult GetRecentOrders()
    {
        return Ok(_dashboardService.GetRecentOrders());
    }

    [HttpGet("products/popular")]
    public IActionResult GetPopularProducts()
    {
        return Ok(_dashboardService.GetPopularProducts());
    }
}
