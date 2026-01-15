using Ecommarce.Api.Models;

namespace Ecommarce.Api.Services;

public interface IBlogPostService
{
    (List<BlogPost> Items, int Total) FilterBlogPosts(string? searchTerm, string category, int page, int pageSize);
    BlogPost? GetFeaturedBlogPost();
    BlogPost? GetBlogPostBySlug(string slug);
    List<BlogPost> GetRelatedBlogPosts(string slug, int limit);
}
