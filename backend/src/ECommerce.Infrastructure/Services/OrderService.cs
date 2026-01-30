using ECommerce.Core.DTOs;
using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Services;

public class OrderService
{
    private readonly ApplicationDbContext _context;

    public OrderService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OrderDto> CreateOrderAsync(OrderCreateDto orderDto)
    {
        var items = new List<OrderItem>();
        
        foreach (var itemDto in orderDto.Items)
        {
            var product = await _context.Products.FindAsync(itemDto.ProductId);
            
            if (product == null) continue; // Or throw error
            
            // In a real app, update stock here
            var orderItem = new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.SalePrice ?? product.Price, // Use backend price for security
                Quantity = itemDto.Quantity,
                Color = itemDto.Color,
                Size = itemDto.Size
            };
            
            items.Add(orderItem);
        }

        var subtotal = items.Sum(i => i.TotalPrice);
        
        var order = new Order
        {
            OrderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}",
            CustomerName = orderDto.Name,
            CustomerPhone = orderDto.Phone,
            ShippingAddress = orderDto.Address,
            DeliveryDetails = orderDto.DeliveryDetails,
            Items = items,
            SubTotal = subtotal,
            Tax = subtotal * 0.08m, // Example tax
            ShippingCost = subtotal > 5000 ? 0 : 120, // Example shipping logic matching frontend
            Status = OrderStatus.Confirmed
        };
        
        order.Total = order.SubTotal + order.Tax + order.ShippingCost;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return MapToDto(order);
    }

    public async Task<List<OrderDto>> GetOrdersAsync()
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
            
        return orders.Select(MapToDto).ToList();
    }

    private OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerName = order.CustomerName,
            CustomerPhone = order.CustomerPhone,
            ShippingAddress = order.ShippingAddress,
            DeliveryDetails = order.DeliveryDetails,
            Total = order.Total,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice,
                Color = i.Color,
                Size = i.Size
            }).ToList()
        };
    }
}
