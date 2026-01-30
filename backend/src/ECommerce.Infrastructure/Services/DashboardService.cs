using ECommerce.Core.DTOs;
using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var totalOrders = await _context.Orders.CountAsync();
        var totalProducts = await _context.Products.CountAsync();
        var totalCustomers = await _context.Users.CountAsync();
        // Calculate total sales from completed orders
        var totalSales = await _context.Orders
            .Where(o => o.Status == OrderStatus.Delivered || o.Status == OrderStatus.Shipped) 
            .SumAsync(o => (int)o.SubTotal);

        return new DashboardStatsDto
        {
            TotalOrders = totalOrders,
            TotalProducts = totalProducts,
            TotalCustomers = totalCustomers,
            TotalSales = totalSales
        };
    }

    public async Task<List<PopularProductDto>> GetPopularProductsAsync()
    {
        var products = await _context.Products
            .Include(p => p.Images)
            .Take(5)
            .ToListAsync();

        return products.Select(p => new PopularProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Stock = p.Stock,
            SoldCount = 0, // Placeholder sold count
            ImageUrl = p.ImageUrl ?? p.Images.FirstOrDefault()?.Url ?? ""
        }).ToList();
    }

    public async Task<List<RecentOrderDto>> GetRecentOrdersAsync()
    {
        return await _context.Orders
            .OrderByDescending(o => o.CreatedAt)
            .Take(5)
            .Select(o => new RecentOrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerName = o.CustomerName,
                OrderDate = o.CreatedAt,
                Total = o.Total,
                Status = o.Status.ToString(),
                PaymentStatus = "Paid" // Placeholder
            })
            .ToListAsync();
    }
}
