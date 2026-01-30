namespace Ecommarce.Domain.Entities;

using Ecommarce.Domain.Common;

public class ProductEntity : BaseEntity
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Category { get; set; }
    public string SubCategory { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public decimal PurchaseRate { get; set; }
    public decimal BasePrice { get; set; }
    public int Stock { get; set; }
    public string Status { get; set; } = "Draft";
    public bool StatusActive { get; set; }
    public bool Featured { get; set; }
    public bool NewArrival { get; set; }
    public string Gender { get; set; } = string.Empty;
    
    // JSON columns
    public List<string> Tags { get; set; } = new();
    public List<string> Badges { get; set; } = new();
    public List<string> MediaUrls { get; set; } = new();
    public string? ImageUrl { get; set; }
    
    // Complex types stored as JSON
    public ProductRatings Ratings { get; set; } = new();
    public ProductImages Images { get; set; } = new();
    public ProductVariants Variants { get; set; } = new();
    public ProductMeta Meta { get; set; } = new();
    public List<RelatedProductInfo> RelatedProducts { get; set; } = new();
    public List<InventoryVariant> InventoryVariants { get; set; } = new();
}

public class ProductRatings
{
    public decimal AvgRating { get; set; }
    public int ReviewCount { get; set; }
    public List<RatingBreakdownItem> RatingBreakdown { get; set; } = new();
}

public class RatingBreakdownItem
{
    public int Rating { get; set; }
    public int Percentage { get; set; }
}

public class ProductImages
{
    public ProductImage MainImage { get; set; } = new();
    public List<ProductImage> Thumbnails { get; set; } = new();
}

public class ProductImage
{
    public string Type { get; set; } = "image";
    public string Label { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Alt { get; set; } = string.Empty;
}

public class ProductVariants
{
    public List<VariantColor> Colors { get; set; } = new();
    public List<VariantSize> Sizes { get; set; } = new();
}

public class VariantColor
{
    public string Name { get; set; } = string.Empty;
    public string Hex { get; set; } = string.Empty;
    public bool Selected { get; set; }
}

public class VariantSize
{
    public string Label { get; set; } = string.Empty;
    public int Stock { get; set; }
    public bool Selected { get; set; }
}

public class ProductMeta
{
    public string FabricAndCare { get; set; } = string.Empty;
    public string ShippingAndReturns { get; set; } = string.Empty;
}

public class RelatedProductInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}

public class InventoryVariant
{
    public string Label { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Sku { get; set; } = string.Empty;
    public int Inventory { get; set; }
    public string? ImageUrl { get; set; }
}
