using System.Collections.Concurrent;
using Ecommarce.Api.Models;

namespace Ecommarce.Api.Services;

public sealed class CustomerOrderStore
{
    private readonly ConcurrentDictionary<string, CustomerProfile> _profiles = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<CustomerOrder> _orders = [];
    private int _sequence = 1000;

    public CustomerProfile? GetProfile(string phone)
    {
        var normalized = NormalizePhone(phone);
        return _profiles.TryGetValue(normalized, out var profile) ? profile : null;
    }

    public CustomerOrderResponse CreateOrder(CustomerOrderRequest payload)
    {
        var normalizedPhone = NormalizePhone(payload.Phone);
        var profile = new CustomerProfile
        {
            Name = payload.Name.Trim(),
            Phone = normalizedPhone,
            Address = payload.Address.Trim(),
            UpdatedAt = DateTimeOffset.UtcNow
        };
        _profiles[normalizedPhone] = profile;

        var order = new CustomerOrder
        {
            Id = _orders.Count + 1,
            OrderId = $"ORD-{Interlocked.Increment(ref _sequence)}",
            Name = payload.Name.Trim(),
            Phone = normalizedPhone,
            Address = payload.Address.Trim(),
            DeliveryDetails = payload.DeliveryDetails.Trim(),
            ItemsCount = payload.ItemsCount,
            Total = payload.Total,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _orders.Add(order);

        return new CustomerOrderResponse
        {
            OrderId = order.OrderId,
            Name = order.Name,
            Phone = order.Phone,
            Address = order.Address,
            DeliveryDetails = order.DeliveryDetails,
            ItemsCount = order.ItemsCount,
            Total = order.Total,
            CreatedAt = order.CreatedAt
        };
    }

    private static string NormalizePhone(string phone)
    {
        var trimmed = phone.Trim();
        var digits = new string(trimmed.Where(char.IsDigit).ToArray());
        return string.IsNullOrWhiteSpace(digits) ? trimmed : digits;
    }
}
