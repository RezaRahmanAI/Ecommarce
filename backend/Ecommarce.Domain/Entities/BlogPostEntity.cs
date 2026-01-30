namespace Ecommarce.Domain.Entities;

using Ecommarce.Domain.Common;

public class BlogPostEntity : BaseEntity
{
    public required string Slug { get; set; }
    public required string Title { get; set; }
    public required string Excerpt { get; set; }
    public string ContentHtml { get; set; } = string.Empty;
    public List<BlogContentBlock> Content { get; set; } = new();
    public string CoverImage { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorAvatar { get; set; } = string.Empty;
    public string AuthorBio { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public int ReadTime { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool Featured { get; set; }
    public string CoverImageCaption { get; set; } = string.Empty;
}

public class BlogContentBlock
{
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
