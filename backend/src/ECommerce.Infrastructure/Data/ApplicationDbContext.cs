using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ECommerce.Core.Entities;

namespace ECommerce.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Product Configuration
        builder.Entity<Product>(entity =>
        {
            entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            entity.Property(p => p.SalePrice).HasColumnType("decimal(18,2)");
            entity.Property(p => p.PurchaseRate).HasColumnType("decimal(18,2)");
            
            entity.HasOne(p => p.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(p => p.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Order Configuration
        builder.Entity<Order>(entity =>
        {
            entity.Property(o => o.SubTotal).HasColumnType("decimal(18,2)");
            entity.Property(o => o.Tax).HasColumnType("decimal(18,2)");
            entity.Property(o => o.ShippingCost).HasColumnType("decimal(18,2)");
            entity.Property(o => o.Total).HasColumnType("decimal(18,2)");
            
            entity.HasMany(o => o.Items)
                  .WithOne(i => i.Order)
                  .HasForeignKey(i => i.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<OrderItem>(entity =>
        {
            entity.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
            
            entity.HasOne(i => i.Product)
                  .WithMany()
                  .HasForeignKey(i => i.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Category Self-Referencing
        builder.Entity<Category>()
               .HasOne(c => c.Parent)
               .WithMany(c => c.SubCategories)
               .HasForeignKey(c => c.ParentId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
