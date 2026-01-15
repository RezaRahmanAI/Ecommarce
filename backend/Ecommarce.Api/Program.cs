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

    policy.WithOrigins("https://your-domain.example")
      .AllowAnyHeader()
      .AllowAnyMethod();
  });
});

var app = builder.Build();

app.UseCors("AppCors");
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

await ApplyDatabaseMigrationsAsync(app.Services);
await SeedAdminUserAsync(app.Services);

app.MapControllers();

app.Run();

static async Task ApplyDatabaseMigrationsAsync(IServiceProvider services)
{
  await using var scope = services.CreateAsyncScope();
  var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
  await dbContext.Database.MigrateAsync();
}

static async Task SeedAdminUserAsync(IServiceProvider services)
{
  await using var scope = services.CreateAsyncScope();
  var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
  var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
  var settings = scope.ServiceProvider.GetRequiredService<IOptions<AdminUserSettings>>().Value;

  if (string.IsNullOrWhiteSpace(settings.Email) || string.IsNullOrWhiteSpace(settings.Password))
  {
    return;
  }

  if (!await roleManager.RoleExistsAsync("admin"))
  {
    await roleManager.CreateAsync(new IdentityRole("admin"));
  }

  var user = await userManager.FindByEmailAsync(settings.Email);
  if (user is null)
  {
    user = new ApplicationUser
    {
      UserName = settings.Email,
      Email = settings.Email,
      FirstName = settings.FirstName,
      LastName = settings.LastName
    };

    var result = await userManager.CreateAsync(user, settings.Password);
    if (!result.Succeeded)
    {
      return;
    }
  }

  if (!await userManager.IsInRoleAsync(user, "admin"))
  {
    await userManager.AddToRoleAsync(user, "admin");
  }
}
