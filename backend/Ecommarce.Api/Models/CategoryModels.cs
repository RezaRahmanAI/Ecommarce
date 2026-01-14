namespace Ecommarce.Api.Models;

public class Category
{
  public required string Id { get; set; }
  public required string Name { get; set; }
  public required string Slug { get; set; }
  public string? ParentId { get; set; }
  public string? Description { get; set; }
  public string? ImageUrl { get; set; }
  public bool IsVisible { get; set; }
  public int ProductCount { get; set; }
  public int SortOrder { get; set; }
}

public record CategoryNode(Category Category, List<CategoryNode> Children);

public record ReorderPayload(string? ParentId, List<string> OrderedIds);

public class CategoryPayload
{
  public required string Name { get; set; }
  public required string Slug { get; set; }
  public string? ParentId { get; set; }
  public string? Description { get; set; }
  public string? ImageUrl { get; set; }
  public bool? IsVisible { get; set; }
  public int? ProductCount { get; set; }
  public int? SortOrder { get; set; }
}
