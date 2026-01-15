using Ecommarce.Api.Models;

namespace Ecommarce.Api.Services;

public interface IAdminBlogService
{
    List<BlogPost> GetBlogPosts();
    BlogPost? GetBlogPost(int id);
    BlogPost CreateBlogPost(BlogPostPayload payload);
    BlogPost? UpdateBlogPost(int id, BlogPostPayload payload);
    bool DeleteBlogPost(int id);
}
