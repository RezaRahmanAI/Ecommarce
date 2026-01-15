using System.ComponentModel.DataAnnotations;

namespace Ecommarce.Api.Models;

public sealed class CustomerProfile
{
    public required string Name { get; set; }
    public required string Phone { get; set; }
    public required string Address { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public sealed class CustomerOrder
{
    public int Id { get; set; }
    public required string OrderId { get; set; }
    public required string Name { get; set; }
    public required string Phone { get; set; }
    public required string Address { get; set; }
    public string DeliveryDetails { get; set; } = string.Empty;
    public int ItemsCount { get; set; }
    public decimal Total { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public sealed class CustomerOrderRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    [Required]
    public string DeliveryDetails { get; set; } = string.Empty;

    public int ItemsCount { get; set; }

    public decimal Total { get; set; }
}

public sealed class CustomerOrderResponse
{
    public required string OrderId { get; set; }
    public required string Name { get; set; }
    public required string Phone { get; set; }
    public required string Address { get; set; }
    public required string DeliveryDetails { get; set; }
    public int ItemsCount { get; set; }
    public decimal Total { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public sealed class CustomerLookupResponse
{
    public required string Name { get; set; }
    public required string Phone { get; set; }
    public required string Address { get; set; }
}
