namespace ECommerce.Core.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Sku { get; set; }
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public int Stock { get; set; }
    public string Status { get; set; }
    public string? ImageUrl { get; set; }
    public string Category { get; set; }
    public int CategoryId { get; set; }
    
    // Additional fields for frontend
    public List<string> MediaUrls { get; set; } = new List<string>();
    public ProductImagesDto Images { get; set; }
}

public class ProductImagesDto
{
    public ImageDto MainImage { get; set; }
    public List<ImageDto> Thumbnails { get; set; } = new List<ImageDto>();
}

public class ImageDto
{
    public string Url { get; set; }
    public string Alt { get; set; }
    public string Label { get; set; }
    public string Type { get; set; } = "image";
}
