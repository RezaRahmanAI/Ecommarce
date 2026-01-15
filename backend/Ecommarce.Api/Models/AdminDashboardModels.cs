namespace Ecommarce.Api.Models;

public record DashboardStats(
  decimal TotalRevenue,
  string TotalOrders,
  string DeliveredOrders,
  string OrdersLeft,
  string ReturnedOrders,
  string CustomerQueries
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
