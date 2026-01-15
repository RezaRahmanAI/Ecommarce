namespace Ecommarce.Api.Models;

public class BlogPost
{
  public int Id { get; set; }
  public required string Slug { get; set; }
  public required string Title { get; set; }
  public required string Excerpt { get; set; }
  public string? ContentHtml { get; set; }
  public List<BlogContentBlock> Content { get; set; } = [];
  public required string CoverImage { get; set; }
  public required string Category { get; set; }
  public required string AuthorName { get; set; }
  public required string AuthorAvatar { get; set; }
  public string? AuthorBio { get; set; }
  public DateTime PublishedAt { get; set; }
  public required string ReadTime { get; set; }
  public List<string> Tags { get; set; } = [];
  public bool Featured { get; set; }
  public string? CoverImageCaption { get; set; }
}

public class BlogContentBlock
{
  public required string Type { get; set; }
  public string? Text { get; set; }
  public int? ProductId { get; set; }
  public string? ProductName { get; set; }
  public string? ProductDescription { get; set; }
  public string? ProductImage { get; set; }
  public string? ProductImageAlt { get; set; }
}

public class BlogPostPayload
{
  public required string Slug { get; set; }
  public required string Title { get; set; }
  public required string Excerpt { get; set; }
  public string? ContentHtml { get; set; }
  public List<BlogContentBlock> Content { get; set; } = [];
  public required string CoverImage { get; set; }
  public required string Category { get; set; }
  public required string AuthorName { get; set; }
  public required string AuthorAvatar { get; set; }
  public string? AuthorBio { get; set; }
  public DateTime PublishedAt { get; set; }
  public required string ReadTime { get; set; }
  public List<string> Tags { get; set; } = [];
  public bool Featured { get; set; }
  public string? CoverImageCaption { get; set; }
}
