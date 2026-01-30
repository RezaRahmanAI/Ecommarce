namespace Ecommarce.Domain.Entities;

using Ecommarce.Domain.Common;

public class CategoryEntity : BaseEntity
{
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public string? ParentId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsVisible { get; set; } = true;
    public int ProductCount { get; set; }
    public int SortOrder { get; set; }
}
