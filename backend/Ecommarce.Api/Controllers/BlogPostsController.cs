using Ecommarce.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace Ecommarce.Api.Controllers;

[ApiController]
[Route("api/blog/posts")]
public sealed class BlogPostsController : ControllerBase
{
    private readonly AdminDataStore _store;

    public BlogPostsController(AdminDataStore store)
    {
        _store = store;
    }

    [HttpGet]
    public IActionResult GetPosts(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 6)
    {
        var (items, total) = _store.FilterBlogPosts(search ?? string.Empty, category ?? string.Empty, page, pageSize);
        return Ok(new { posts = items, total, page, pageSize });
    }

    [HttpGet("featured")]
    public IActionResult GetFeaturedPost()
    {
        var post = _store.GetFeaturedBlogPost();
        return post is null ? NotFound() : Ok(post);
    }

    [HttpGet("{slug}")]
    public IActionResult GetPost(string slug)
    {
        var post = _store.GetBlogPostBySlug(slug);
        return post is null ? NotFound() : Ok(post);
    }

    [HttpGet("{slug}/related")]
    public IActionResult GetRelatedPosts(string slug, [FromQuery] int limit = 3)
    {
        return Ok(_store.GetRelatedBlogPosts(slug, limit));
    }
}
