using Ecommarce.Api.Models;

namespace Ecommarce.Api.Services;

public interface IAdminCatalogService
{
    List<Category> GetCategories();
    List<CategoryNode> GetCategoryTree();
    Category? GetCategory(string id);
    Category CreateCategory(CategoryPayload payload);
    Category? UpdateCategory(string id, CategoryPayload payload);
    bool DeleteCategory(string id);
    bool ReorderCategories(ReorderPayload payload);
    List<Product> GetProducts();
    Product? GetProduct(int id);
    Product CreateProduct(ProductCreatePayload payload);
    Product? UpdateProduct(int id, ProductUpdatePayload payload);
    bool DeleteProduct(int id);
    bool RemoveProductMedia(int id, string mediaUrl);
    (List<Product> Items, int Total) FilterProducts(string? searchTerm, string category, string statusTab, int page, int pageSize);
    List<Product> FilterProducts(string? searchTerm, string category, string statusTab);
}
