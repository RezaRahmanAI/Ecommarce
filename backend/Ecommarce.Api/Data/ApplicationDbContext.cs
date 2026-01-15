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
    private const int MoneyPrecision = 18;
    private const int MoneyScale = 2;
    private const int RatingPrecision = 4;
    private const int RatingScale = 2;
    private static readonly ValueConverter<List<string>, string> StringListConverter = new(
        value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
        value => JsonSerializer.Deserialize<List<string>>(value, (JsonSerializerOptions?)null) ?? new List<string>());

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

            entity.Property(product => product.Price)
                .HasPrecision(MoneyPrecision, MoneyScale);

            entity.Property(product => product.SalePrice)
                .HasPrecision(MoneyPrecision, MoneyScale);

            entity.Property(product => product.PurchaseRate)
                .HasPrecision(MoneyPrecision, MoneyScale);

            entity.Property(product => product.BasePrice)
                .HasPrecision(MoneyPrecision, MoneyScale);

            entity.OwnsOne(product => product.Ratings, ratings =>
            {
                ratings.ToJson();
                ratings.Property(item => item.AvgRating)
                    .HasPrecision(RatingPrecision, RatingScale);
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
                related.Property(item => item.Price)
                    .HasPrecision(MoneyPrecision, MoneyScale);
            });

            entity.OwnsMany(product => product.InventoryVariants, inventory =>
            {
                inventory.ToJson();
                inventory.Property(item => item.Price)
                    .HasPrecision(MoneyPrecision, MoneyScale);
            });
        });

        builder.Entity<CustomerOrder>(entity =>
        {
            entity.Property(order => order.Total)
                .HasPrecision(MoneyPrecision, MoneyScale);
        });

        builder.Entity<Order>(entity =>
        {
            entity.Property(order => order.Total)
                .HasPrecision(MoneyPrecision, MoneyScale);
        });
    }
}
