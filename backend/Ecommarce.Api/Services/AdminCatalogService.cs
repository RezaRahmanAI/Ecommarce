using Ecommarce.Api.Models;
using Ecommarce.Api.Repositories;

namespace Ecommarce.Api.Services;

public sealed class AdminCatalogService : IAdminCatalogService
{
    private readonly IAdminRepository _repository;

    public AdminCatalogService(IAdminRepository repository)
    {
        _repository = repository;
    }

    public List<Category> GetCategories() => _repository.GetCategories();

    public List<CategoryNode> GetCategoryTree() => _repository.GetCategoryTree();

    public Category? GetCategory(string id) => _repository.GetCategory(id);

    public Category CreateCategory(CategoryPayload payload) => _repository.CreateCategory(payload);

    public Category? UpdateCategory(string id, CategoryPayload payload) => _repository.UpdateCategory(id, payload);

    public bool DeleteCategory(string id) => _repository.DeleteCategory(id);

    public bool ReorderCategories(ReorderPayload payload) => _repository.ReorderCategories(payload);

    public List<Product> GetProducts() => _repository.GetProducts();

    public Product? GetProduct(int id) => _repository.GetProduct(id);

    public Product CreateProduct(ProductCreatePayload payload) => _repository.CreateProduct(payload);

    public Product? UpdateProduct(int id, ProductUpdatePayload payload) => _repository.UpdateProduct(id, payload);

    public bool DeleteProduct(int id) => _repository.DeleteProduct(id);

    public bool RemoveProductMedia(int id, string mediaUrl) => _repository.RemoveProductMedia(id, mediaUrl);

    public (List<Product> Items, int Total) FilterProducts(string? searchTerm, string category, string statusTab, int page, int pageSize)
        => _repository.FilterProducts(searchTerm, category, statusTab, page, pageSize);

    public List<Product> FilterProducts(string? searchTerm, string category, string statusTab)
        => _repository.FilterProducts(searchTerm, category, statusTab);
}
