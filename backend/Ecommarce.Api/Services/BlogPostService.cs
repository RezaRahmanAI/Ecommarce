using Ecommarce.Api.Models;
using Ecommarce.Api.Repositories;

namespace Ecommarce.Api.Services;

public sealed class BlogPostService : IBlogPostService
{
    private readonly IAdminRepository _repository;

    public BlogPostService(IAdminRepository repository)
    {
        _repository = repository;
    }

    public (List<BlogPost> Items, int Total) FilterBlogPosts(string? searchTerm, string category, int page, int pageSize)
        => _repository.FilterBlogPosts(searchTerm, category, page, pageSize);

    public BlogPost? GetFeaturedBlogPost() => _repository.GetFeaturedBlogPost();

    public BlogPost? GetBlogPostBySlug(string slug) => _repository.GetBlogPostBySlug(slug);

    public List<BlogPost> GetRelatedBlogPosts(string slug, int limit) => _repository.GetRelatedBlogPosts(slug, limit);
}
