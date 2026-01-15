using Ecommarce.Api.Models;

namespace Ecommarce.Api.Services;

public interface IOrderService
{
    CustomerOrderResponse CreateOrder(CustomerOrderRequest payload);
}
