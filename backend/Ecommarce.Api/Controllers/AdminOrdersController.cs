using Ecommarce.Api.Models;
using Ecommarce.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommarce.Api.Controllers;

[ApiController]
[Route("api/admin/orders")]
public sealed class AdminOrdersController : ControllerBase
{
    private readonly IAdminOrderService _orderService;

    public AdminOrdersController(IAdminOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public IActionResult GetOrders(
        [FromQuery] string? searchTerm,
        [FromQuery] string? status,
        [FromQuery] string? dateRange,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var filtered = _orderService.FilterOrders(searchTerm ?? string.Empty, status ?? string.Empty, dateRange ?? string.Empty);
        var items = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Ok(new { items, total = filtered.Count });
    }

    [HttpGet("filtered")]
    public IActionResult GetFilteredOrders(
        [FromQuery] string? searchTerm,
        [FromQuery] string? status,
        [FromQuery] string? dateRange)
    {
        var filtered = _orderService.FilterOrders(searchTerm ?? string.Empty, status ?? string.Empty, dateRange ?? string.Empty);
        return Ok(filtered);
    }

    [HttpPost]
    public IActionResult CreateOrder(OrderCreatePayload payload)
    {
        return Ok(_orderService.CreateOrder(payload));
    }

    [HttpPut("{id:int}/status")]
    public IActionResult UpdateOrderStatus(int id, OrderStatusUpdate payload)
    {
        var order = _orderService.UpdateOrderStatus(id, payload.Status);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteOrder(int id)
    {
        return Ok(_orderService.DeleteOrder(id));
    }
}
