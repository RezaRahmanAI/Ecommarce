using Ecommarce.Api.Models;
using Ecommarce.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommarce.Api.Controllers;

[ApiController]
[Route("api/admin/dashboard")]
public sealed class AdminDashboardController : ControllerBase
{
    private readonly IAdminDashboardService _dashboardService;
    private readonly IImageStorageService _imageStorage;

    public AdminDashboardController(IAdminDashboardService dashboardService, IImageStorageService imageStorage)
    {
        _dashboardService = dashboardService;
        _imageStorage = imageStorage;
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
        var products = _dashboardService.GetPopularProducts()
            .Select(product => new PopularProduct(
                product.Id,
                product.Name,
                product.SoldCount,
                product.Price,
                _imageStorage.BuildPublicUrl(Request, product.ImageUrl) ?? product.ImageUrl
            ))
            .ToList();
        return Ok(products);
    }
}
