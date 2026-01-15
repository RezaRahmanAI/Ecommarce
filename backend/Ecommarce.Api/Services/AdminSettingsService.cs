using Ecommarce.Api.Models;
using Ecommarce.Api.Repositories;

namespace Ecommarce.Api.Services;

public sealed class AdminSettingsService : IAdminSettingsService
{
    private readonly IAdminRepository _repository;

    public AdminSettingsService(IAdminRepository repository)
    {
        _repository = repository;
    }

    public AdminSettings GetSettings() => _repository.GetSettings();

    public AdminSettings SaveSettings(AdminSettings payload) => _repository.SaveSettings(payload);

    public ShippingZone CreateShippingZone(ShippingZone payload) => _repository.CreateShippingZone(payload);

    public ShippingZone? UpdateShippingZone(int id, ShippingZone payload) => _repository.UpdateShippingZone(id, payload);

    public bool DeleteShippingZone(int id) => _repository.DeleteShippingZone(id);
}
