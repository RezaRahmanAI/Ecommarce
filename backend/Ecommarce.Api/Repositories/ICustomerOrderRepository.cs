using Ecommarce.Api.Models;

namespace Ecommarce.Api.Repositories;

public interface ICustomerOrderRepository
{
    CustomerProfile? GetProfile(string phone);
    CustomerOrderResponse CreateOrder(CustomerOrderRequest payload);
}
