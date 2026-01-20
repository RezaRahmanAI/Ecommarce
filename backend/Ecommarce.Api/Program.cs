using System.Text.Json.Serialization;
using Ecommarce.Api.Data;
using Ecommarce.Api.Middleware;
using Ecommarce.Api.Models;
using Ecommarce.Api.Repositories;
using Ecommarce.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IAdminRepository, AdminRepository>();
builder.Services.AddSingleton<ICustomerOrderRepository, CustomerOrderRepository>();
builder.Services.AddSingleton<IImageStorageService, ImageStorageService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentityCore<ApplicationUser>(options =>
  {
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
  })
  .AddRoles<IdentityRole>()
  .AddEntityFrameworkStores<ApplicationDbContext>()
  .AddSignInManager();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
builder.Services.Configure<AdminUserSettings>(builder.Configuration.GetSection(AdminUserSettings.SectionName));
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAdminCatalogService, AdminCatalogService>();
builder.Services.AddScoped<IAdminOrderService, AdminOrderService>();
builder.Services.AddScoped<IAdminBlogService, AdminBlogService>();
builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();
builder.Services.AddScoped<IAdminSettingsService, AdminSettingsService>();
builder.Services.AddScoped<IBlogPostService, BlogPostService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddControllers();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.ConfigureHttpJsonOptions(options =>
{
  options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
  options.AddPolicy("AppCors", policy =>
  {
    if (builder.Environment.IsDevelopment())
    {
      policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
      return;
    }

    policy.WithOrigins(
        "http://localhost:4200",
            "https://cecom.octimsbd.com",
            "http://cecom.octimsbd.com",
            "http://cecom.octimsbd.com",
            "https://cecom.octimsbd.com")
      .AllowAnyHeader()
      .AllowAnyMethod();
  });
});

var app = builder.Build();

app.UseCors("AppCors");
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

await ApplyDatabaseMigrationsAsync(app.Services);

app.MapControllers();

app.Run();

static async Task ApplyDatabaseMigrationsAsync(IServiceProvider services)
{
  await using var scope = services.CreateAsyncScope();
  var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
  await dbContext.Database.MigrateAsync();
  await SeedAdminUserAsync(scope.ServiceProvider);
}

static async Task SeedAdminUserAsync(IServiceProvider services)
{
  var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
  var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
  var settings = services.GetRequiredService<IOptions<AdminUserSettings>>().Value;

  if (string.IsNullOrWhiteSpace(settings.Email) || string.IsNullOrWhiteSpace(settings.Password))
  {
    return;
  }

  var adminRole = "admin";
  if (!await roleManager.RoleExistsAsync(adminRole))
  {
    await roleManager.CreateAsync(new IdentityRole(adminRole));
  }

  var desiredUsername = string.IsNullOrWhiteSpace(settings.Username)
    ? settings.Email
    : settings.Username;
  var user = await userManager.FindByEmailAsync(settings.Email);
  if (user is null && !string.IsNullOrWhiteSpace(settings.Username))
  {
    user = await userManager.FindByNameAsync(settings.Username);
  }

  if (user is null)
  {
    user = new ApplicationUser
    {
      UserName = desiredUsername,
      Email = settings.Email,
      FirstName = settings.FirstName,
      LastName = settings.LastName,
      EmailConfirmed = true
    };

    var result = await userManager.CreateAsync(user, settings.Password);
    if (!result.Succeeded)
    {
      return;
    }
  }
  else
  {
    var shouldUpdate = false;
    if (!string.Equals(user.UserName, desiredUsername, StringComparison.Ordinal))
    {
      user.UserName = desiredUsername;
      shouldUpdate = true;
    }

    if (!string.Equals(user.Email, settings.Email, StringComparison.OrdinalIgnoreCase))
    {
      user.Email = settings.Email;
      shouldUpdate = true;
    }

    if (!string.Equals(user.FirstName, settings.FirstName, StringComparison.Ordinal))
    {
      user.FirstName = settings.FirstName;
      shouldUpdate = true;
    }

    if (!string.Equals(user.LastName, settings.LastName, StringComparison.Ordinal))
    {
      user.LastName = settings.LastName;
      shouldUpdate = true;
    }

    if (!user.EmailConfirmed)
    {
      user.EmailConfirmed = true;
      shouldUpdate = true;
    }

    if (shouldUpdate)
    {
      var updateResult = await userManager.UpdateAsync(user);
      if (!updateResult.Succeeded)
      {
        return;
      }
    }

    if (!await userManager.CheckPasswordAsync(user, settings.Password))
    {
      var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
      var resetResult = await userManager.ResetPasswordAsync(user, resetToken, settings.Password);
      if (!resetResult.Succeeded)
      {
        return;
      }
    }
  }

  if (!await userManager.IsInRoleAsync(user, adminRole))
  {
    await userManager.AddToRoleAsync(user, adminRole);
  }
}
