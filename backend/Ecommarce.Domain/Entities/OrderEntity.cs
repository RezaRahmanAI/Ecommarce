namespace Ecommarce.Domain.Entities;

using Ecommarce.Domain.Common;

public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled,
    Refund
}

public class OrderEntity : BaseEntity
{
    public required string  OrderId { get; set; }
    public required string CustomerName { get; set; }
    public string CustomerInitials { get; set; } = string.Empty;
    public required string Date { get; set; }
    public int ItemsCount { get; set; }
    public decimal Total { get; set; }
    public string DeliveryDetails { get; set; } = string.Empty;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string? UserId { get; set; } // For future user linking
}
