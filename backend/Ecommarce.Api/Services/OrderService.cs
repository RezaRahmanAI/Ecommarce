using System.Globalization;
using Ecommarce.Api.Models;
using Ecommarce.Api.Repositories;

namespace Ecommarce.Api.Services;

public sealed class OrderService : IOrderService
{
    private readonly ICustomerOrderRepository _customerOrders;
    private readonly IAdminRepository _adminRepository;

    public OrderService(ICustomerOrderRepository customerOrders, IAdminRepository adminRepository)
    {
        _customerOrders = customerOrders;
        _adminRepository = adminRepository;
    }

    public CustomerOrderResponse CreateOrder(CustomerOrderRequest payload)
    {
        if (string.IsNullOrWhiteSpace(payload.Name))
        {
            throw new ArgumentException("Customer name is required.", nameof(payload));
        }

        var response = _customerOrders.CreateOrder(payload);
        var initials = string.Join(string.Empty, payload.Name
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Take(2)
            .Select(part => char.ToUpperInvariant(part[0])));

        _adminRepository.CreateOrder(new OrderCreatePayload
        {
            OrderId = response.OrderId,
            CustomerName = payload.Name.Trim(),
            CustomerInitials = string.IsNullOrWhiteSpace(initials) ? "NA" : initials,
            Date = DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            ItemsCount = payload.ItemsCount,
            Total = payload.Total,
            DeliveryDetails = payload.DeliveryDetails,
            Status = OrderStatus.Processing
        });

        return response;
    }
}
