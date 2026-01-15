using Ecommarce.Api.Data;
using Ecommarce.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecommarce.Api.Controllers;

[ApiController]
[Route("api/admin/blog/posts")]
public sealed class AdminBlogPostsController : ControllerBase
{
    private readonly AdminDataStore _store;

    public AdminBlogPostsController(AdminDataStore store)
    {
        _store = store;
    }

    [HttpGet]
    public IActionResult GetPosts()
    {
        return Ok(_store.GetBlogPosts());
    }

    [HttpGet("{id:int}")]
    public IActionResult GetPost(int id)
    {
        var post = _store.GetBlogPost(id);
        return post is null ? NotFound() : Ok(post);
    }

    [HttpPost]
    public IActionResult CreatePost(BlogPostPayload payload)
    {
        return Ok(_store.CreateBlogPost(payload));
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdatePost(int id, BlogPostPayload payload)
    {
        var post = _store.UpdateBlogPost(id, payload);
        return post is null ? NotFound() : Ok(post);
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeletePost(int id)
    {
        return Ok(_store.DeleteBlogPost(id));
    }
}
