using Ecommarce.Api.Models;

namespace Ecommarce.Api.Services;

public interface IAdminSettingsService
{
    AdminSettings GetSettings();
    AdminSettings SaveSettings(AdminSettings payload);
    ShippingZone CreateShippingZone(ShippingZone payload);
    ShippingZone? UpdateShippingZone(int id, ShippingZone payload);
    bool DeleteShippingZone(int id);
}
