namespace ECommerce.Core.DTOs;

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Sku { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public decimal? PurchaseRate { get; set; }
    public int Stock { get; set; }
    public string Status { get; set; } = "Active";
    public int CategoryId { get; set; }
}

public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Sku { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public decimal? PurchaseRate { get; set; }
    public int Stock { get; set; }
    public string Status { get; set; } = "Active";
    public int CategoryId { get; set; }
}
