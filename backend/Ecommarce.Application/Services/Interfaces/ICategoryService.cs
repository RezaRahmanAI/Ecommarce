namespace Ecommarce.Application.Services.Interfaces;

using Ecommarce.Application.Features.Categories.DTOs;
using Ecommarce.Application.Features.Categories.Queries.GetCategories;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetCategoriesAsync();
}
