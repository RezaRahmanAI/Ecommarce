using Ecommarce.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace Ecommarce.Api.Controllers;

[ApiController]
[Route("api/admin/dashboard")]
public sealed class AdminDashboardController : ControllerBase
{
    private readonly AdminDataStore _store;

    public AdminDashboardController(AdminDataStore store)
    {
        _store = store;
    }

    [HttpGet("stats")]
    public IActionResult GetStats()
    {
        return Ok(_store.GetDashboardStats());
    }

    [HttpGet("orders/recent")]
    public IActionResult GetRecentOrders()
    {
        return Ok(_store.GetRecentOrders());
    }

    [HttpGet("products/popular")]
    public IActionResult GetPopularProducts()
    {
        return Ok(_store.GetPopularProducts());
    }
}
