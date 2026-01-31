namespace ECommerce.Core.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Sku { get; set; }
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public decimal? PurchaseRate { get; set; }
    public int Stock { get; set; }
    public string Status { get; set; }
    public string? ImageUrl { get; set; }
    public string Category { get; set; }
    public int CategoryId { get; set; }
    
    // Additional product details
    public string? SubCategory { get; set; }
    public string? Gender { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> Badges { get; set; } = new();
    public bool Featured { get; set; }
    public bool NewArrival { get; set; }
    
    // Media
    public ProductImagesDto Images { get; set; }
    
    // Variants
    public ProductVariantsDto Variants { get; set; } = new();
    
    // Meta
    public ProductMetaDto Meta { get; set; } = new();
    
    // Ratings
    public ProductRatingsDto Ratings { get; set; } = new();
    
    // Related products (empty for now)
    public List<RelatedProductDto> RelatedProducts { get; set; } = new();
}

public class ProductImagesDto
{
    public ImageDto MainImage { get; set; }
    public List<ImageDto> Thumbnails { get; set; } = new();
}

public class ImageDto
{
    public string Url { get; set; }
    public string Alt { get; set; }
    public string Label { get; set; }
    public string Type { get; set; } = "image";
}

public class ProductVariantsDto
{
    public List<ColorDto> Colors { get; set; } = new();
    public List<SizeDto> Sizes { get; set; } = new();
}

public class ColorDto
{
    public string Name { get; set; }
    public string Hex { get; set; }
    public bool Selected { get; set; }
}

public class SizeDto
{
    public string Label { get; set; }
    public int Stock { get; set; }
    public bool Selected { get; set; }
}

public class ProductMetaDto
{
    public string FabricAndCare { get; set; } = "";
    public string ShippingAndReturns { get; set; } = "";
}

public class ProductRatingsDto
{
    public double AvgRating { get; set; } = 0;
    public int ReviewCount { get; set; } = 0;
    public List<RatingBreakdownDto> RatingBreakdown { get; set; } = new();
}

public class RatingBreakdownDto
{
    public int Rating { get; set; }
    public int Percentage { get; set; }
}

public class RelatedProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public ImageDto Image { get; set; }
    public string QuickViewLabel { get; set; } = "Quick View";
}
