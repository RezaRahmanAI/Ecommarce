namespace ECommerce.Core.DTOs;

// Main product creation DTO
public class ProductCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool StatusActive { get; set; } = true;
    public string Category { get; set; } = string.Empty;
    public string? SubCategory { get; set; }
    public string Gender { get; set; } = "women";
    public List<string> Badges { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public decimal PurchaseRate { get; set; }
    public bool Featured { get; set; }
    public bool NewArrival { get; set; }
    public ProductMediaDto Media { get; set; } = new();
    public ProductVariantsDto Variants { get; set; } = new();
    public ProductMetaDto Meta { get; set; } = new();
    public ProductRatingsDto Ratings { get; set; } = new();
}

// Product update DTO
public class ProductUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool StatusActive { get; set; } = true;
    public string Category { get; set; } = string.Empty;
    public string? SubCategory { get; set; }
    public string Gender { get; set; } = "women";
    public List<string> Badges { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public decimal PurchaseRate { get; set; }
    public bool Featured { get; set; }
    public bool NewArrival { get; set; }
    public ProductMediaDto Media { get; set; } = new();
    public ProductVariantsDto Variants { get; set; } = new();
    public ProductMetaDto Meta { get; set; } = new();
}

// Supporting DTOs
public class ProductMediaDto
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

public class ProductColorDto
{
    public string Name { get; set; } = string.Empty;
    public string Hex { get; set; } = "#111827";
    public bool Selected { get; set; }
}

public class ProductSizeDto
{
    public string Label { get; set; } = string.Empty;
    public int Stock { get; set; }
    public bool Selected { get; set; }
}
