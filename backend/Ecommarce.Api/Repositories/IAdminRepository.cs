using Ecommarce.Api.Models;

namespace Ecommarce.Api.Repositories;

public interface IAdminRepository
{
    List<Category> GetCategories();
    Category? GetCategory(string id);
    Category CreateCategory(CategoryPayload payload);
    Category? UpdateCategory(string id, CategoryPayload payload);
    bool DeleteCategory(string id);
    bool ReorderCategories(ReorderPayload payload);
    List<CategoryNode> GetCategoryTree();
    List<Product> GetProducts();
    Product? GetProduct(int id);
    Product CreateProduct(ProductCreatePayload payload);
    Product? UpdateProduct(int id, ProductUpdatePayload payload);
    bool DeleteProduct(int id);
    bool RemoveProductMedia(int id, string mediaUrl);
    List<Order> GetOrders();
    Order CreateOrder(OrderCreatePayload payload);
    Order? UpdateOrderStatus(int id, OrderStatus status);
    bool DeleteOrder(int id);
    List<BlogPost> GetBlogPosts();
    BlogPost? GetBlogPost(int id);
    BlogPost? GetBlogPostBySlug(string slug);
    BlogPost? GetFeaturedBlogPost();
    BlogPost CreateBlogPost(BlogPostPayload payload);
    BlogPost? UpdateBlogPost(int id, BlogPostPayload payload);
    bool DeleteBlogPost(int id);
    (List<BlogPost> Items, int Total) FilterBlogPosts(string? searchTerm, string category, int page, int pageSize);
    List<BlogPost> FilterBlogPosts(string? searchTerm, string category);
    List<BlogPost> GetRelatedBlogPosts(string slug, int limit);
    AdminSettings GetSettings();
    AdminSettings SaveSettings(AdminSettings payload);
    ShippingZone CreateShippingZone(ShippingZone payload);
    ShippingZone? UpdateShippingZone(int id, ShippingZone payload);
    bool DeleteShippingZone(int id);
    List<Order> FilterOrders(string? searchTerm, string status, string dateRange);
    (List<Product> Items, int Total) FilterProducts(string? searchTerm, string category, string statusTab, int page, int pageSize);
    List<Product> FilterProducts(string? searchTerm, string category, string statusTab);
    DashboardStats GetDashboardStats();
    List<OrderItem> GetRecentOrders();
    List<PopularProduct> GetPopularProducts();
}
