using Ecommarce.Api.Models;

namespace Ecommarce.Api.Services;

public interface ICustomerService
{
    CustomerProfile? GetProfile(string phone);
}
