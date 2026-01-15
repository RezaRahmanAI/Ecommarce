using Ecommarce.Api.Models;
using Ecommarce.Api.Repositories;

namespace Ecommarce.Api.Services;

public sealed class AdminBlogService : IAdminBlogService
{
    private readonly IAdminRepository _repository;

    public AdminBlogService(IAdminRepository repository)
    {
        _repository = repository;
    }

    public List<BlogPost> GetBlogPosts() => _repository.GetBlogPosts();

    public BlogPost? GetBlogPost(int id) => _repository.GetBlogPost(id);

    public BlogPost CreateBlogPost(BlogPostPayload payload) => _repository.CreateBlogPost(payload);

    public BlogPost? UpdateBlogPost(int id, BlogPostPayload payload) => _repository.UpdateBlogPost(id, payload);

    public bool DeleteBlogPost(int id) => _repository.DeleteBlogPost(id);
}
