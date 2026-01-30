namespace Ecommarce.Application.Features.Categories.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? ParentId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsVisible { get; set; }
    public int ProductCount { get; set; }
    public int SortOrder { get; set; }
}
