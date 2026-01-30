namespace Ecommarce.Application.Common.Interfaces;

using Ecommarce.Domain.Entities;

public interface IApplicationDbContext
{
    IQueryable<ProductEntity> Products { get; }
    IQueryable<CategoryEntity> Categories { get; }
    IQueryable<OrderEntity> Orders { get; }
    IQueryable<BlogPostEntity> BlogPosts { get; }
    IQueryable<CartEntity> Carts { get; }
    IQueryable<CartItemEntity> CartItems { get; }
    
    void AddProduct(ProductEntity product);
    void AddCategory(CategoryEntity category);
    void AddOrder(OrderEntity order);
    void AddBlogPost(BlogPostEntity blogPost);
    void AddCart(CartEntity cart);
    void AddCartItem(CartItemEntity cartItem);
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
