using Ecommarce.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommarce.Api.Controllers;

[ApiController]
[Route("api/blog/posts")]
public sealed class BlogPostsController : ControllerBase
{
    private readonly IBlogPostService _blogPostService;

    public BlogPostsController(IBlogPostService blogPostService)
    {
        _blogPostService = blogPostService;
    }

    [HttpGet]
    public IActionResult GetPosts(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 6)
    {
        var (items, total) = _blogPostService.FilterBlogPosts(search ?? string.Empty, category ?? string.Empty, page, pageSize);
        return Ok(new { posts = items, total, page, pageSize });
    }

    [HttpGet("featured")]
    public IActionResult GetFeaturedPost()
    {
        var post = _blogPostService.GetFeaturedBlogPost();
        return post is null ? NotFound() : Ok(post);
    }

    [HttpGet("{slug}")]
    public IActionResult GetPost(string slug)
    {
        var post = _blogPostService.GetBlogPostBySlug(slug);
        return post is null ? NotFound() : Ok(post);
    }

    [HttpGet("{slug}/related")]
    public IActionResult GetRelatedPosts(string slug, [FromQuery] int limit = 3)
    {
        return Ok(_blogPostService.GetRelatedBlogPosts(slug, limit));
    }
}
