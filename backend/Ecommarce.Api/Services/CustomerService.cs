using Ecommarce.Api.Models;
using Ecommarce.Api.Repositories;

namespace Ecommarce.Api.Services;

public sealed class CustomerService : ICustomerService
{
    private readonly ICustomerOrderRepository _repository;

    public CustomerService(ICustomerOrderRepository repository)
    {
        _repository = repository;
    }

    public CustomerProfile? GetProfile(string phone) => _repository.GetProfile(phone);
}
