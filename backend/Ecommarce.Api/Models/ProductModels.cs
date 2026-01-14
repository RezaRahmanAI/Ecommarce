namespace Ecommarce.Api.Models;

public class Product
{
  public int Id { get; set; }
  public required string Name { get; set; }
  public required string Description { get; set; }
  public required string Category { get; set; }
  public required string SubCategory { get; set; }
  public List<string> Tags { get; set; } = [];
  public List<string> Badges { get; set; } = [];
  public decimal Price { get; set; }
  public decimal? SalePrice { get; set; }
  public required string Gender { get; set; }
  public required ProductRatings Ratings { get; set; }
  public required ProductImages Images { get; set; }
  public required ProductVariants Variants { get; set; }
  public required ProductMeta Meta { get; set; }
  public List<RelatedProduct> RelatedProducts { get; set; } = [];
  public bool Featured { get; set; }
  public bool NewArrival { get; set; }
  public required string Sku { get; set; }
  public int Stock { get; set; }
  public required string Status { get; set; }
  public string? ImageUrl { get; set; }
  public bool StatusActive { get; set; }
  public List<string> MediaUrls { get; set; } = [];
  public decimal BasePrice { get; set; }
  public List<ProductVariantEdit> InventoryVariants { get; set; } = [];
}

public record ProductRatings(decimal AvgRating, int ReviewCount, List<RatingBreakdown> RatingBreakdown);

public record RatingBreakdown(int Rating, int Percentage);

public record ProductImages(ProductImage MainImage, List<ProductImage> Thumbnails);

public record ProductImage(string Type, string Label, string Url, string Alt);

public record ProductVariants(List<VariantColor> Colors, List<VariantSize> Sizes);

public record VariantColor(string Name, string Hex, bool Selected);

public record VariantSize(string Label, int Stock, bool Selected);

public record ProductMeta(string FabricAndCare, string ShippingAndReturns);

public record RelatedProduct(int Id, string Name, decimal Price, string ImageUrl);

public record ProductVariantEdit(string Label, decimal Price, string Sku, int Inventory, string? ImageUrl);

public class ProductCreatePayload
{
  public required string Name { get; set; }
  public required string Description { get; set; }
  public bool StatusActive { get; set; }
  public required string Category { get; set; }
  public required string SubCategory { get; set; }
  public required string Gender { get; set; }
  public List<string> Tags { get; set; } = [];
  public List<string> Badges { get; set; } = [];
  public decimal Price { get; set; }
  public decimal? SalePrice { get; set; }
  public bool Featured { get; set; }
  public bool NewArrival { get; set; }
  public required ProductRatings Ratings { get; set; }
  public required ProductImages Media { get; set; }
  public required ProductVariants Variants { get; set; }
  public required ProductMeta Meta { get; set; }
}

public class ProductUpdatePayload
{
  public required string Name { get; set; }
  public required string Description { get; set; }
  public bool StatusActive { get; set; }
  public required string Category { get; set; }
  public string? SubCategory { get; set; }
  public required string Gender { get; set; }
  public List<string> Tags { get; set; } = [];
  public List<string> Badges { get; set; } = [];
  public bool Featured { get; set; }
  public bool NewArrival { get; set; }
  public decimal BasePrice { get; set; }
  public decimal? SalePrice { get; set; }
  public List<string> MediaUrls { get; set; } = [];
  public List<ProductVariantEdit> InventoryVariants { get; set; } = [];
}

public record ProductMediaRemovePayload(string MediaUrl);
