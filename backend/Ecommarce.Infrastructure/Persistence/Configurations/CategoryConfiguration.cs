namespace Ecommarce.Infrastructure.Persistence.Configurations;

using Ecommarce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CategoryConfiguration : IEntityTypeConfiguration<CategoryEntity>
{
    public void Configure(EntityTypeBuilder<CategoryEntity> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Slug)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(c => c.Slug)
            .IsUnique();

        builder.HasIndex(c => c.ParentId);
        
        builder.HasIndex(c => c.SortOrder);

        builder.Property(c => c.Description)
            .HasMaxLength(500);
    }
}
