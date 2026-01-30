namespace Ecommarce.Infrastructure.Persistence.Configurations;

using Ecommarce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class OrderConfiguration : IEntityTypeConfiguration<OrderEntity>
{
    private const int MoneyPrecision = 18;
    private const int MoneyScale = 2;

    public void Configure(EntityTypeBuilder<OrderEntity> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderId)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(o => o.OrderId)
            .IsUnique();

        builder.Property(o => o.CustomerName)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(o => o.Status);
        
        builder.HasIndex(o => o.Date);

        builder.Property(o => o.Total)
            .HasPrecision(MoneyPrecision, MoneyScale);

        builder.Property(o => o.Status)
            .HasConversion<string>();
    }
}
