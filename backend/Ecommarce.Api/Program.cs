using System.Text.Json.Serialization;
using Ecommarce.Api.Data;
using Ecommarce.Api.Models;
using Ecommarce.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<AdminDataStore>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
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

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

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

app.Run();
