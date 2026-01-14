namespace Ecommarce.Api.Models;

public record DashboardStats(
  decimal TotalRevenue,
  string RevenueTrend,
  string NewOrders,
  string OrdersTrend,
  string ActiveCustomers,
  string CustomersTrend,
  string LowStock,
  string LowStockTrend
);

public record OrderItem(
  string Id,
  string CustomerName,
  string Date,
  decimal Amount,
  string Status
);

public record PopularProduct(
  string Id,
  string Name,
  string SoldCount,
  decimal Price,
  string ImageUrl
);
