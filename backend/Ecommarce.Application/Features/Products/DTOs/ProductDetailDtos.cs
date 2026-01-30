namespace Ecommarce.Application.Features.Products.DTOs;

public class ProductRatingsDto
{
    public decimal AvgRating { get; set; }
    public int ReviewCount { get; set; }
    public List<RatingBreakdownItemDto> RatingBreakdown { get; set; } = new();
}

public class RatingBreakdownItemDto
{
    public int Rating { get; set; }
    public int Percentage { get; set; }
}

public class ProductImagesDto
{
    public ProductImageDto MainImage { get; set; } = new();
    public List<ProductImageDto> Thumbnails { get; set; } = new();
}

public class ProductImageDto
{
    public string Type { get; set; } = "image";
    public string Label { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Alt { get; set; } = string.Empty;
}

public class ProductVariantsDto
{
    public List<VariantColorDto> Colors { get; set; } = new();
    public List<VariantSizeDto> Sizes { get; set; } = new();
}

public class VariantColorDto
{
    public string Name { get; set; } = string.Empty;
    public string Hex { get; set; } = string.Empty;
    public bool Selected { get; set; }
}

public class VariantSizeDto
{
    public string Label { get; set; } = string.Empty;
    public int Stock { get; set; }
    public bool Selected { get; set; }
}

public class ProductMetaDto
{
    public string FabricAndCare { get; set; } = string.Empty;
    public string ShippingAndReturns { get; set; } = string.Empty;
}

public class RelatedProductInfoDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}
