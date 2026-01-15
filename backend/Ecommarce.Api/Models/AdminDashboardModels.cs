namespace Ecommarce.Api.Models;

public record DashboardStats(
  decimal TotalRevenue,
  string TotalOrders,
  string DeliveredOrders,
  string OrdersLeft,
  string ReturnedOrders,
  string CustomerQueries,
  decimal TotalPurchaseCost,
  decimal AverageSellingPrice,
  decimal ReturnValue,
  string ReturnRate
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
