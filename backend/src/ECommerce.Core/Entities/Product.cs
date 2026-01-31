using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Core.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Sku { get; set; }
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public decimal? PurchaseRate { get; set; }
    public int Stock { get; set; }
    public string Status { get; set; } = "Active"; // Active, Draft, Archived
    
    public string? ImageUrl { get; set; } // Main image
    
    // Additional product details
    public string? SubCategory { get; set; }
    public string? Gender { get; set; } // men, women, kids, accessories
    public string? Tags { get; set; } // JSON array stored as string
    public string? Badges { get; set; } // JSON array stored as string
    public bool Featured { get; set; } = false;
    public bool NewArrival { get; set; } = false;
    
    // Variants (colors and sizes) stored as JSON
    public string? Variants { get; set; } // JSON object
    
    // Meta information stored as JSON
    public string? Meta { get; set; } // JSON object with fabricAndCare and shippingAndReturns
    
    // Foreign Key
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
