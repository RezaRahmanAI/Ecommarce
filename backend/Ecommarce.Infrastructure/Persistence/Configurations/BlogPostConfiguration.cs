namespace Ecommarce.Infrastructure.Persistence.Configurations;

using System.Text.Json;
using Ecommarce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPostEntity>
{
    public void Configure(EntityTypeBuilder<BlogPostEntity> builder)
    {
        builder.ToTable("BlogPosts");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(b => b.Slug)
            .IsRequired()
            .HasMaxLength(350);

        builder.HasIndex(b => b.Slug)
            .IsUnique();

        builder.HasIndex(b => b.Category);
        
        builder.HasIndex(b => b.PublishedAt);
        
        builder.HasIndex(b => b.Featured);

        builder.Property(b => b.Excerpt)
            .IsRequired()
            .HasMaxLength(500);

        // JSON columns
        var stringListConverter = new ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());

        var stringListComparer = new ValueComparer<List<string>>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

        builder.Property(b => b.Tags)
            .HasConversion(stringListConverter)
            .Metadata.SetValueComparer(stringListComparer);

        builder.OwnsMany(b => b.Content, content =>
        {
            content.ToJson();
        });
    }
}
