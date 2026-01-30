namespace Ecommarce.Infrastructure.Persistence.Configurations;

using System.Text.Json;
using Ecommarce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class ProductConfiguration : IEntityTypeConfiguration<ProductEntity>
{
    private const int MoneyPrecision = 18;
    private const int MoneyScale = 2;
    private const int RatingPrecision = 4;
    private const int RatingScale = 2;

    private static List<string> DeserializeListSafe(string v)
    {
        if (string.IsNullOrEmpty(v))
        {
            return new List<string>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    public void Configure(EntityTypeBuilder<ProductEntity> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(p => p.Sku)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.Sku)
            .IsUnique();

        builder.HasIndex(p => p.Category);
        
        builder.HasIndex(p => new { p.Category, p.Status });
        
        builder.HasIndex(p => p.Featured);

        // Money properties
        builder.Property(p => p.Price)
            .HasPrecision(MoneyPrecision, MoneyScale);

        builder.Property(p => p.SalePrice)
            .HasPrecision(MoneyPrecision, MoneyScale);

        builder.Property(p => p.PurchaseRate)
            .HasPrecision(MoneyPrecision, MoneyScale);

        builder.Property(p => p.BasePrice)
            .HasPrecision(MoneyPrecision, MoneyScale);

        // JSON columns for lists
        var stringListConverter = new ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => DeserializeListSafe(v));

        var stringListComparer = new ValueComparer<List<string>>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

        builder.Property(p => p.Tags)
            .HasConversion(stringListConverter)
            .Metadata.SetValueComparer(stringListComparer);

        builder.Property(p => p.Badges)
            .HasConversion(stringListConverter)
            .Metadata.SetValueComparer(stringListComparer);

        builder.Property(p => p.MediaUrls)
            .HasConversion(stringListConverter)
            .Metadata.SetValueComparer(stringListComparer);

        // Complex types as JSON
        builder.OwnsOne(p => p.Ratings, ratings =>
        {
            ratings.ToJson();
            ratings.Property(r => r.AvgRating)
                .HasPrecision(RatingPrecision, RatingScale);
            ratings.OwnsMany(r => r.RatingBreakdown);
        });

        builder.OwnsOne(p => p.Images, images =>
        {
            images.ToJson();
            images.OwnsOne(i => i.MainImage);
            images.OwnsMany(i => i.Thumbnails);
        });

        builder.OwnsOne(p => p.Variants, variants =>
        {
            variants.ToJson();
            variants.OwnsMany(v => v.Colors);
            variants.OwnsMany(v => v.Sizes);
        });

        builder.OwnsOne(p => p.Meta, meta =>
        {
            meta.ToJson();
        });

        builder.OwnsMany(p => p.RelatedProducts, related =>
        {
            related.ToJson();
            related.Property(r => r.Price)
                .HasPrecision(MoneyPrecision, MoneyScale);
        });

        builder.OwnsMany(p => p.InventoryVariants, inventory =>
        {
            inventory.ToJson();
            inventory.Property(i => i.Price)
                .HasPrecision(MoneyPrecision, MoneyScale);
        });
    }
}
