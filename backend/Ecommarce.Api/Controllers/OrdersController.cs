using System.Globalization;
using Ecommarce.Api.Data;
using Ecommarce.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecommarce.Api.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly CustomerOrderStore _store;
    private readonly AdminDataStore _adminStore;

    public OrdersController(CustomerOrderStore store, AdminDataStore adminStore)
    {
        _store = store;
        _adminStore = adminStore;
    }

    [HttpPost]
    public IActionResult CreateOrder(CustomerOrderRequest payload)
    {
        var response = _store.CreateOrder(payload);
        var initials = string.Join(string.Empty, payload.Name
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Take(2)
            .Select(part => char.ToUpperInvariant(part[0])));

        _adminStore.CreateOrder(new OrderCreatePayload
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

        return Ok(response);
    }
}
