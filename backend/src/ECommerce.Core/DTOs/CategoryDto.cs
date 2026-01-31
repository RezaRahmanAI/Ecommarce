namespace ECommerce.Core.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int? ParentId { get; set; }
    public bool IsVisible { get; set; }
    public int SortOrder { get; set; }
    public int ProductCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CategoryCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? ImageUrl { get; set; }
    public int? ParentId { get; set; }
    public bool? IsVisible { get; set; }
    public int? SortOrder { get; set; }
}

public class CategoryUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? ImageUrl { get; set; }
    public int? ParentId { get; set; }
    public bool? IsVisible { get; set; }
    public int? SortOrder { get; set; }
}

public class ReorderCategoriesDto
{
    public int? ParentId { get; set; }
    public List<int> OrderedIds { get; set; } = new();
}


