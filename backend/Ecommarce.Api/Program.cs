using System.Text.Json.Serialization;
using Ecommarce.Api.Data;
using Ecommarce.Api.Models;
using Ecommarce.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<AdminDataStore>();
builder.Services.AddSingleton<CustomerOrderStore>();
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
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

await SeedAdminUserAsync(app.Services);

app.MapGet("/api/admin/dashboard/stats", (AdminDataStore store) => Results.Ok(store.GetDashboardStats()));
app.MapGet("/api/admin/dashboard/orders/recent", (AdminDataStore store) => Results.Ok(store.GetRecentOrders()));
app.MapGet("/api/admin/dashboard/products/popular", (AdminDataStore store) => Results.Ok(store.GetPopularProducts()));

app.MapGet("/api/admin/categories", (AdminDataStore store) => Results.Ok(store.GetCategories()));
app.MapGet("/api/admin/categories/tree", (AdminDataStore store) => Results.Ok(store.GetCategoryTree()));
app.MapGet("/api/admin/categories/{id}", (string id, AdminDataStore store) =>
{
  var category = store.GetCategory(id);
  return category is null ? Results.NotFound() : Results.Ok(category);
});
app.MapPost("/api/admin/categories", (CategoryPayload payload, AdminDataStore store) =>
  Results.Ok(store.CreateCategory(payload)));
app.MapPut("/api/admin/categories/{id}", (string id, CategoryPayload payload, AdminDataStore store) =>
{
  var category = store.UpdateCategory(id, payload);
  return category is null ? Results.NotFound() : Results.Ok(category);
});
app.MapDelete("/api/admin/categories/{id}", (string id, AdminDataStore store) =>
  Results.Ok(store.DeleteCategory(id)));
app.MapPost("/api/admin/categories/reorder", (ReorderPayload payload, AdminDataStore store) =>
  Results.Ok(store.ReorderCategories(payload)));
app.MapPost("/api/admin/categories/image", async (HttpRequest request) =>
{
  if (!request.HasFormContentType)
  {
    return Results.BadRequest("Expected multipart form data.");
  }

  var form = await request.ReadFormAsync();
  var file = form.Files.FirstOrDefault();
  if (file is null || file.Length == 0)
  {
    return Results.BadRequest("No file uploaded.");
  }

  await using var stream = new MemoryStream();
  await file.CopyToAsync(stream);
  var base64 = Convert.ToBase64String(stream.ToArray());
  var dataUrl = $"data:{file.ContentType};base64,{base64}";
  return Results.Ok(dataUrl);
});

app.MapGet("/api/admin/products/catalog", (AdminDataStore store) => Results.Ok(store.GetProducts()));
app.MapGet("/api/admin/products", (HttpRequest request, AdminDataStore store) =>
{
  var searchTerm = request.Query["searchTerm"].ToString();
  var category = request.Query["category"].ToString();
  var statusTab = request.Query["statusTab"].ToString();
  var page = int.TryParse(request.Query["page"], out var parsedPage) ? parsedPage : 1;
  var pageSize = int.TryParse(request.Query["pageSize"], out var parsedPageSize) ? parsedPageSize : 10;

  var (items, total) = store.FilterProducts(searchTerm, category, statusTab, page, pageSize);
  return Results.Ok(new { items, total });
});
app.MapGet("/api/admin/products/filtered", (HttpRequest request, AdminDataStore store) =>
{
  var searchTerm = request.Query["searchTerm"].ToString();
  var category = request.Query["category"].ToString();
  var statusTab = request.Query["statusTab"].ToString();
  var items = store.FilterProducts(searchTerm, category, statusTab);
  return Results.Ok(items);
});
app.MapGet("/api/admin/products/{id:int}", (int id, AdminDataStore store) =>
{
  var product = store.GetProduct(id);
  return product is null ? Results.NotFound() : Results.Ok(product);
});
app.MapPost("/api/admin/products", (ProductCreatePayload payload, AdminDataStore store) =>
  Results.Ok(store.CreateProduct(payload)));
app.MapPut("/api/admin/products/{id:int}", (int id, ProductUpdatePayload payload, AdminDataStore store) =>
{
  var product = store.UpdateProduct(id, payload);
  return product is null ? Results.NotFound() : Results.Ok(product);
});
app.MapDelete("/api/admin/products/{id:int}", (int id, AdminDataStore store) =>
  Results.Ok(store.DeleteProduct(id)));
app.MapPost("/api/admin/products/media", async (HttpRequest request) =>
{
  if (!request.HasFormContentType)
  {
    return Results.BadRequest("Expected multipart form data.");
  }

  var form = await request.ReadFormAsync();
  var files = form.Files;
  if (files.Count == 0)
  {
    return Results.Ok(Array.Empty<string>());
  }

  var results = new List<string>();
  foreach (var file in files)
  {
    await using var stream = new MemoryStream();
    await file.CopyToAsync(stream);
    var base64 = Convert.ToBase64String(stream.ToArray());
    results.Add($"data:{file.ContentType};base64,{base64}");
  }

  return Results.Ok(results);
});
app.MapPost("/api/admin/products/{id:int}/media/remove", (int id, ProductMediaRemovePayload payload, AdminDataStore store) =>
  Results.Ok(store.RemoveProductMedia(id, payload.MediaUrl)));

app.MapGet("/api/admin/orders", (HttpRequest request, AdminDataStore store) =>
{
  var searchTerm = request.Query["searchTerm"].ToString();
  var status = request.Query["status"].ToString();
  var dateRange = request.Query["dateRange"].ToString();
  var page = int.TryParse(request.Query["page"], out var parsedPage) ? parsedPage : 1;
  var pageSize = int.TryParse(request.Query["pageSize"], out var parsedPageSize) ? parsedPageSize : 10;

  var filtered = store.FilterOrders(searchTerm, status, dateRange);
  var items = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();
  return Results.Ok(new { items, total = filtered.Count });
});
app.MapGet("/api/admin/orders/filtered", (HttpRequest request, AdminDataStore store) =>
{
  var searchTerm = request.Query["searchTerm"].ToString();
  var status = request.Query["status"].ToString();
  var dateRange = request.Query["dateRange"].ToString();
  var filtered = store.FilterOrders(searchTerm, status, dateRange);
  return Results.Ok(filtered);
});
app.MapPost("/api/admin/orders", (OrderCreatePayload payload, AdminDataStore store) =>
  Results.Ok(store.CreateOrder(payload)));
app.MapPut("/api/admin/orders/{id:int}/status", (int id, OrderStatusUpdate payload, AdminDataStore store) =>
{
  var order = store.UpdateOrderStatus(id, payload.Status);
  return order is null ? Results.NotFound() : Results.Ok(order);
});
app.MapDelete("/api/admin/orders/{id:int}", (int id, AdminDataStore store) =>
  Results.Ok(store.DeleteOrder(id)));

app.MapGet("/api/admin/blog/posts", (AdminDataStore store) => Results.Ok(store.GetBlogPosts()));
app.MapGet("/api/admin/blog/posts/{id:int}", (int id, AdminDataStore store) =>
{
  var post = store.GetBlogPost(id);
  return post is null ? Results.NotFound() : Results.Ok(post);
});
app.MapPost("/api/admin/blog/posts", (BlogPostPayload payload, AdminDataStore store) =>
  Results.Ok(store.CreateBlogPost(payload)));
app.MapPut("/api/admin/blog/posts/{id:int}", (int id, BlogPostPayload payload, AdminDataStore store) =>
{
  var post = store.UpdateBlogPost(id, payload);
  return post is null ? Results.NotFound() : Results.Ok(post);
});
app.MapDelete("/api/admin/blog/posts/{id:int}", (int id, AdminDataStore store) =>
  Results.Ok(store.DeleteBlogPost(id)));

app.MapGet("/api/blog/posts", (HttpRequest request, AdminDataStore store) =>
{
  var search = request.Query["search"].ToString();
  var category = request.Query["category"].ToString();
  var page = int.TryParse(request.Query["page"], out var parsedPage) ? parsedPage : 1;
  var pageSize = int.TryParse(request.Query["pageSize"], out var parsedPageSize) ? parsedPageSize : 6;

  var (items, total) = store.FilterBlogPosts(search, category, page, pageSize);
  return Results.Ok(new { posts = items, total, page, pageSize });
});
app.MapGet("/api/blog/posts/featured", (AdminDataStore store) =>
{
  var post = store.GetFeaturedBlogPost();
  return post is null ? Results.NotFound() : Results.Ok(post);
});
app.MapGet("/api/blog/posts/{slug}", (string slug, AdminDataStore store) =>
{
  var post = store.GetBlogPostBySlug(slug);
  return post is null ? Results.NotFound() : Results.Ok(post);
});
app.MapGet("/api/blog/posts/{slug}/related", (string slug, HttpRequest request, AdminDataStore store) =>
{
  var limit = int.TryParse(request.Query["limit"], out var parsedLimit) ? parsedLimit : 3;
  return Results.Ok(store.GetRelatedBlogPosts(slug, limit));
});

app.MapGet("/api/admin/settings", (AdminDataStore store) => Results.Ok(store.GetSettings()));
app.MapPut("/api/admin/settings", (AdminSettings payload, AdminDataStore store) => Results.Ok(store.SaveSettings(payload)));
app.MapPost("/api/admin/settings/shipping-zones", (ShippingZone payload, AdminDataStore store) =>
  Results.Ok(store.CreateShippingZone(payload)));
app.MapPut("/api/admin/settings/shipping-zones/{id:int}", (int id, ShippingZone payload, AdminDataStore store) =>
{
  var zone = store.UpdateShippingZone(id, payload);
  return zone is null ? Results.NotFound() : Results.Ok(zone);
});
app.MapDelete("/api/admin/settings/shipping-zones/{id:int}", (int id, AdminDataStore store) =>
  Results.Ok(store.DeleteShippingZone(id)));

app.MapGet("/api/customers/lookup", (string phone, CustomerOrderStore store) =>
{
  if (string.IsNullOrWhiteSpace(phone))
  {
    return Results.BadRequest("Phone number is required.");
  }

  var profile = store.GetProfile(phone);
  if (profile is null)
  {
    return Results.NotFound();
  }

  return Results.Ok(new CustomerLookupResponse
  {
    Name = profile.Name,
    Phone = profile.Phone,
    Address = profile.Address
  });
});

app.MapPost("/api/orders", (CustomerOrderRequest payload, CustomerOrderStore store) =>
  Results.Ok(store.CreateOrder(payload)));

app.MapControllers();

app.Run();

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
