using Ecommarce.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommarce.Api.Data;

public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<DashboardWidget> DashboardWidgets => Set<DashboardWidget>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<DashboardWidget>(entity =>
        {
            entity.HasKey(widget => widget.Id);
            entity.Property(widget => widget.Title).HasMaxLength(200).IsRequired();
            entity.Property(widget => widget.Value).HasMaxLength(500).IsRequired();
            entity.HasOne(widget => widget.User)
                .WithMany(user => user.DashboardWidgets)
                .HasForeignKey(widget => widget.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
