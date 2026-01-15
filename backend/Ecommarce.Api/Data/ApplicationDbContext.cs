using System.Text.Json;
using System.Linq;
using Ecommarce.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ecommarce.Api.Data;

public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private static readonly ValueConverter<List<string>, string> StringListConverter = new(
        value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
        value => JsonSerializer.Deserialize<List<string>>(value, (JsonSerializerOptions?)null) ?? []);

    private static readonly ValueComparer<List<string>> StringListComparer = new(
        (left, right) => left.SequenceEqual(right),
        value => value.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
        value => value.ToList());

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CustomerOrder> CustomerOrders => Set<CustomerOrder>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<BlogPost>(entity =>
        {
            entity.Property(post => post.Tags)
                .HasConversion(StringListConverter)
                .Metadata.SetValueComparer(StringListComparer);

            entity.OwnsMany(post => post.Content, content =>
            {
                content.ToJson();
            });
        });

        builder.Entity<Product>(entity =>
        {
            entity.Property(product => product.Tags)
                .HasConversion(StringListConverter)
                .Metadata.SetValueComparer(StringListComparer);

            entity.Property(product => product.Badges)
                .HasConversion(StringListConverter)
                .Metadata.SetValueComparer(StringListComparer);

            entity.Property(product => product.MediaUrls)
                .HasConversion(StringListConverter)
                .Metadata.SetValueComparer(StringListComparer);

            entity.OwnsOne(product => product.Ratings, ratings =>
            {
                ratings.ToJson();
                ratings.OwnsMany(item => item.RatingBreakdown);
            });

            entity.OwnsOne(product => product.Images, images =>
            {
                images.ToJson();
                images.OwnsOne(item => item.MainImage);
                images.OwnsMany(item => item.Thumbnails);
            });

            entity.OwnsOne(product => product.Variants, variants =>
            {
                variants.ToJson();
                variants.OwnsMany(item => item.Colors);
                variants.OwnsMany(item => item.Sizes);
            });

            entity.OwnsOne(product => product.Meta, meta =>
            {
                meta.ToJson();
            });

            entity.OwnsMany(product => product.RelatedProducts, related =>
            {
                related.ToJson();
            });

            entity.OwnsMany(product => product.InventoryVariants, inventory =>
            {
                inventory.ToJson();
            });
        });
    }
}
