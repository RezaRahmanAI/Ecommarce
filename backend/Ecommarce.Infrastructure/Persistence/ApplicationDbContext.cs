namespace Ecommarce.Infrastructure.Persistence;

using System.Text.Json;
using Ecommarce.Application.Common.Interfaces;
using Ecommarce.Domain.Entities;
using Ecommarce.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public IQueryable<ProductEntity> Products => Set<ProductEntity>();
    public IQueryable<CategoryEntity> Categories => Set<CategoryEntity>();
    public IQueryable<OrderEntity> Orders => Set<OrderEntity>();
    public IQueryable<BlogPostEntity> BlogPosts => Set<BlogPostEntity>();
    public IQueryable<CartEntity> Carts => Set<CartEntity>();
    public IQueryable<CartItemEntity> CartItems => Set<CartItemEntity>();

    public void AddProduct(ProductEntity product) => Set<ProductEntity>().Add(product);
    public void AddCategory(CategoryEntity category) => Set<CategoryEntity>().Add(category);
    public void AddOrder(OrderEntity order) => Set<OrderEntity>().Add(order);
    public void AddBlogPost(BlogPostEntity blogPost) => Set<BlogPostEntity>().Add(blogPost);
    public void AddCart(CartEntity cart) => Set<CartEntity>().Add(cart);
    public void AddCartItem(CartItemEntity cartItem) => Set<CartItemEntity>().Add(cartItem);

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new ProductConfiguration());
        builder.ApplyConfiguration(new CategoryConfiguration());
        builder.ApplyConfiguration(new OrderConfiguration());
        builder.ApplyConfiguration(new BlogPostConfiguration());
        builder.ApplyConfiguration(new CartConfiguration());
        builder.ApplyConfiguration(new CartItemConfiguration());

        // Global query filter for soft delete
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(Domain.Common.BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(ApplicationDbContext)
                    .GetMethod(nameof(SetGlobalQueryFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                    .MakeGenericMethod(entityType.ClrType);
                method.Invoke(this, new object[] { builder });
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps before saving
        var entries = ChangeTracker.Entries<Domain.Common.BaseEntity>();
        
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void SetGlobalQueryFilter<TEntity>(ModelBuilder builder) where TEntity : Domain.Common.BaseEntity
    {
        builder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
    }
}
