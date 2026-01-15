namespace Ecommarce.Api.Models;

public enum OrderStatus
{
  Processing,
  Shipped,
  Delivered,
  Cancelled,
  Refund
}

public class Order
{
  public int Id { get; set; }
  public required string OrderId { get; set; }
  public required string CustomerName { get; set; }
  public required string CustomerInitials { get; set; }
  public required string Date { get; set; }
  public int ItemsCount { get; set; }
  public decimal Total { get; set; }
  public string DeliveryDetails { get; set; } = string.Empty;
  public OrderStatus Status { get; set; }
}

public record OrderStatusUpdate(OrderStatus Status);

public class OrderCreatePayload
{
  public required string OrderId { get; set; }
  public required string CustomerName { get; set; }
  public required string CustomerInitials { get; set; }
  public required string Date { get; set; }
  public int ItemsCount { get; set; }
  public decimal Total { get; set; }
  public string DeliveryDetails { get; set; } = string.Empty;
  public OrderStatus Status { get; set; }
}
