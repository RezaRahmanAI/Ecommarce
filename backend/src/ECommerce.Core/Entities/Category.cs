using System.Collections.Generic;

namespace ECommerce.Core.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; }
    public string Slug { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsVisible { get; set; } = true;
    public int SortOrder { get; set; }
    
    public int? ParentId { get; set; }
    public Category? Parent { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
