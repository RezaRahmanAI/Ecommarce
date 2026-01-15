using Ecommarce.Api.Models;
using Ecommarce.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommarce.Api.Controllers;

[ApiController]
[Route("api/admin/blog/posts")]
public sealed class AdminBlogPostsController : ControllerBase
{
    private readonly IAdminBlogService _blogService;

    public AdminBlogPostsController(IAdminBlogService blogService)
    {
        _blogService = blogService;
    }

    [HttpGet]
    public IActionResult GetPosts()
    {
        return Ok(_blogService.GetBlogPosts());
    }

    [HttpGet("{id:int}")]
    public IActionResult GetPost(int id)
    {
        var post = _blogService.GetBlogPost(id);
        return post is null ? NotFound() : Ok(post);
    }

    [HttpPost]
    public IActionResult CreatePost(BlogPostPayload payload)
    {
        return Ok(_blogService.CreateBlogPost(payload));
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdatePost(int id, BlogPostPayload payload)
    {
        var post = _blogService.UpdateBlogPost(id, payload);
        return post is null ? NotFound() : Ok(post);
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeletePost(int id)
    {
        return Ok(_blogService.DeleteBlogPost(id));
    }
}
