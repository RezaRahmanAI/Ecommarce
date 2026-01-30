using Microsoft.AspNetCore.Identity;
using ECommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        // Seed Admin User
        if (!await userManager.Users.AnyAsync())
        {
            var adminUser = new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                FullName = "Admin User",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(adminUser, "Admin123");
            // If we had roles, we would add to Admin role here
        }
        
        // Seed Categories
        if (!await context.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new Category { Name = "Men", Slug = "men", SortOrder = 1 },
                new Category { Name = "Women", Slug = "women", SortOrder = 2 },
                new Category { Name = "Kids", Slug = "kids", SortOrder = 3 }
            };
            
            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }
    }
}
