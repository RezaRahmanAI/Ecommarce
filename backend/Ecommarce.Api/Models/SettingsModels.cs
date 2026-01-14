namespace Ecommarce.Api.Models;

public class ShippingZone
{
  public int Id { get; set; }
  public required string Name { get; set; }
  public required string Region { get; set; }
  public List<string> Rates { get; set; } = [];
}

public class AdminSettings
{
  public required string StoreName { get; set; }
  public required string SupportEmail { get; set; }
  public required string Description { get; set; }
  public bool StripeEnabled { get; set; }
  public bool PaypalEnabled { get; set; }
  public required string StripePublishableKey { get; set; }
  public List<ShippingZone> ShippingZones { get; set; } = [];
}
