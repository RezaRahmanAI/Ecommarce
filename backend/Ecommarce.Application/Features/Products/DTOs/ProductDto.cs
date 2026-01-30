namespace Ecommarce.Application.Features.Products.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string SubCategory { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public List<string> Badges { get; set; } = new();
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public decimal PurchaseRate { get; set; }
    public decimal BasePrice { get; set; }
    public string Gender { get; set; } = string.Empty;
    public bool Featured { get; set; }
    public bool NewArrival { get; set; }
    public string Sku { get; set; } = string.Empty;
    public int Stock { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool StatusActive { get; set; }
    public List<string> MediaUrls { get; set; } = new();
}
