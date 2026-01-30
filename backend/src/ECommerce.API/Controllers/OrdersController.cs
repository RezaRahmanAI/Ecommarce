using ECommerce.Core.DTOs;
using ECommerce.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult<dynamic>> CreateOrder(OrderCreateDto orderDto)
    {
        var order = await _orderService.CreateOrderAsync(orderDto);
        
        // Return format expected by frontend:
        // { orderId: string, name, phone, address, deliveryDetails, itemsCount, total, createdAt }
        return Ok(new 
        {
            orderId = order.Id.ToString(), // Or OrderNumber depending on what frontend expects as 'id'
            name = order.CustomerName,
            phone = order.CustomerPhone,
            address = order.ShippingAddress,
            deliveryDetails = order.DeliveryDetails,
            itemsCount = order.Items.Sum(i => i.Quantity),
            total = order.Total,
            createdAt = order.CreatedAt
        });
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetOrders()
    {
        return Ok(await _orderService.GetOrdersAsync());
    }
}
