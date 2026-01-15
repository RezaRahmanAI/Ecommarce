using System.Globalization;
using Ecommarce.Api.Models;
using Ecommarce.Api.Services;

namespace Ecommarce.Api.Repositories;

public sealed class AdminRepository : IAdminRepository
{
  private readonly object _sync = new();
  private readonly IImageStorageService _imageStorage;
  private readonly List<Category> _categories;
  private readonly List<Product> _products;
  private readonly List<Order> _orders;
  private readonly List<BlogPost> _blogPosts;
  private AdminSettings _settings;

  public AdminRepository(IImageStorageService imageStorage)
  {
    _imageStorage = imageStorage;
    _categories = SeedCategories();
    _products = SeedProducts();
    _orders = SeedOrders();
    _blogPosts = SeedBlogPosts();
    _settings = SeedSettings();
  }

  public List<Category> GetCategories()
  {
    lock (_sync)
    {
      return _categories.Select(CloneCategory).ToList();
    }
  }

  public Category? GetCategory(string id)
  {
    lock (_sync)
    {
      var category = _categories.FirstOrDefault(item => item.Id == id);
      return category is null ? null : CloneCategory(category);
    }
  }

  public Category CreateCategory(CategoryPayload payload)
  {
    lock (_sync)
    {
      var parentId = payload.ParentId;
      var siblings = _categories.Where(item => item.ParentId == parentId).ToList();
      var nextSortOrder = siblings.Count > 0 ? siblings.Max(item => item.SortOrder) + 1 : 1;

      var category = new Category
      {
        Id = $"cat-{Guid.NewGuid():N}",
        Name = payload.Name,
        Slug = payload.Slug,
        ParentId = parentId,
        Description = payload.Description,
        ImageUrl = _imageStorage.NormalizeImageName(payload.ImageUrl),
        IsVisible = payload.IsVisible ?? true,
        ProductCount = payload.ProductCount ?? 0,
        SortOrder = payload.SortOrder ?? nextSortOrder
      };

      _categories.Add(category);
      return CloneCategory(category);
    }
  }

  public Category? UpdateCategory(string id, CategoryPayload payload)
  {
    lock (_sync)
    {
      var index = _categories.FindIndex(item => item.Id == id);
      if (index < 0)
      {
        return null;
      }

      var existing = _categories[index];
      existing.Name = payload.Name;
      existing.Slug = payload.Slug;
      existing.ParentId = payload.ParentId;
      existing.Description = payload.Description;
      existing.ImageUrl = _imageStorage.NormalizeImageName(payload.ImageUrl);
      existing.IsVisible = payload.IsVisible ?? existing.IsVisible;
      existing.SortOrder = payload.SortOrder ?? existing.SortOrder;

      return CloneCategory(existing);
    }
  }

  public bool DeleteCategory(string id)
  {
    lock (_sync)
    {
      var removed = _categories.RemoveAll(item => item.Id == id);
      return removed > 0;
    }
  }

  public bool ReorderCategories(ReorderPayload payload)
  {
    lock (_sync)
    {
      foreach (var category in _categories)
      {
        if ((category.ParentId ?? null) != (payload.ParentId ?? null))
        {
          continue;
        }

        var index = payload.OrderedIds.IndexOf(category.Id);
        if (index < 0)
        {
          continue;
        }
        category.SortOrder = index + 1;
      }
      return true;
    }
  }

  public List<CategoryNode> GetCategoryTree()
  {
    return BuildCategoryTree(GetCategories());
  }

  public List<Product> GetProducts()
  {
    lock (_sync)
    {
      return _products.Select(CloneProduct).ToList();
    }
  }

  public Product? GetProduct(int id)
  {
    lock (_sync)
    {
      var product = _products.FirstOrDefault(item => item.Id == id);
      return product is null ? null : CloneProduct(product);
    }
  }

  public Product CreateProduct(ProductCreatePayload payload)
  {
    lock (_sync)
    {
      var nextId = _products.Count > 0 ? _products.Max(item => item.Id) + 1 : 1;
      var normalizedMedia = NormalizeProductImages(payload.Media);
      var mediaUrls = BuildMediaUrls(normalizedMedia);
      var mainImageUrl = mediaUrls.FirstOrDefault() ?? normalizedMedia.MainImage.Url;
      var inventoryVariants = BuildInventoryVariants(payload, nextId, mainImageUrl);
      var stock = inventoryVariants.Sum(variant => variant.Inventory);
      var status = ResolveStatus(payload.StatusActive, stock);

      var product = new Product
      {
        Id = nextId,
        Name = payload.Name,
        Description = payload.Description,
        Category = payload.Category,
        SubCategory = payload.SubCategory,
        Tags = payload.Tags,
        Badges = payload.Badges,
        Price = payload.SalePrice ?? payload.Price,
        SalePrice = payload.SalePrice,
        PurchaseRate = payload.PurchaseRate,
        Gender = payload.Gender,
        Ratings = payload.Ratings,
        Images = normalizedMedia,
        Variants = payload.Variants,
        Meta = payload.Meta,
        RelatedProducts = [],
        Featured = payload.Featured,
        NewArrival = payload.NewArrival,
        Sku = FormatSku(nextId),
        Stock = stock,
        Status = status,
        ImageUrl = mainImageUrl,
        StatusActive = payload.StatusActive,
        MediaUrls = mediaUrls,
        BasePrice = payload.Price,
        InventoryVariants = inventoryVariants
      };

      _products.Insert(0, product);
      return CloneProduct(product);
    }
  }

  public Product? UpdateProduct(int id, ProductUpdatePayload payload)
  {
    lock (_sync)
    {
      var index = _products.FindIndex(item => item.Id == id);
      var existing = index >= 0 ? _products[index] : null;
      var inventoryVariants = payload.InventoryVariants.Count > 0
        ? payload.InventoryVariants
        : existing?.InventoryVariants ?? [];
      var stock = inventoryVariants.Sum(variant => variant.Inventory);
      var status = ResolveStatus(payload.StatusActive, stock);
      var mediaUrls = payload.MediaUrls.Count > 0
        ? NormalizeMediaUrls(payload.MediaUrls)
        : existing?.MediaUrls ?? [];
      var images = BuildImagesFromMedia(mediaUrls, payload.Name, existing?.Images);
      var sku = inventoryVariants.Count > 0
        ? inventoryVariants[0].Sku
        : existing?.Sku ?? FormatSku(id);

      var product = new Product
      {
        Id = id,
        Name = payload.Name,
        Description = payload.Description,
        Category = payload.Category,
        SubCategory = payload.SubCategory ?? string.Empty,
        Tags = payload.Tags,
        Badges = payload.Badges,
        Price = payload.SalePrice ?? payload.BasePrice,
        SalePrice = payload.SalePrice,
        PurchaseRate = payload.PurchaseRate,
        Gender = payload.Gender,
        Ratings = existing?.Ratings ?? BuildDefaultRatings(),
        Images = images,
        Variants = existing?.Variants ?? new ProductVariants([], []),
        Meta = existing?.Meta ?? new ProductMeta(string.Empty, string.Empty),
        RelatedProducts = existing?.RelatedProducts ?? [],
        Featured = payload.Featured,
        NewArrival = payload.NewArrival,
        Sku = sku,
        Stock = stock,
        Status = status,
        ImageUrl = mediaUrls.FirstOrDefault(),
        StatusActive = payload.StatusActive,
        MediaUrls = mediaUrls,
        BasePrice = payload.BasePrice,
        InventoryVariants = inventoryVariants
      };

      if (index >= 0)
      {
        _products[index] = product;
      }
      else
      {
        _products.Insert(0, product);
      }

      return CloneProduct(product);
    }
  }

  public bool DeleteProduct(int id)
  {
    lock (_sync)
    {
      var removed = _products.RemoveAll(item => item.Id == id);
      return removed > 0;
    }
  }

  public bool RemoveProductMedia(int id, string mediaUrl)
  {
    lock (_sync)
    {
      var product = _products.FirstOrDefault(item => item.Id == id);
      if (product is null)
      {
        return false;
      }

      var normalizedMediaUrl = _imageStorage.NormalizeImageName(mediaUrl);
      var updatedMedia = product.MediaUrls.Where(url => url != normalizedMediaUrl).ToList();
      product.MediaUrls = updatedMedia;
      product.Images = BuildImagesFromMedia(updatedMedia, product.Name, product.Images);
      if (product.ImageUrl == normalizedMediaUrl)
      {
        product.ImageUrl = updatedMedia.FirstOrDefault();
      }
      return true;
    }
  }

  public List<Order> GetOrders()
  {
    lock (_sync)
    {
      return _orders.Select(CloneOrder).ToList();
    }
  }

  public Order CreateOrder(OrderCreatePayload payload)
  {
    lock (_sync)
    {
      var nextId = _orders.Count > 0 ? _orders.Max(item => item.Id) + 1 : 1;
      var order = new Order
      {
        Id = nextId,
        OrderId = payload.OrderId,
        CustomerName = payload.CustomerName,
        CustomerInitials = payload.CustomerInitials,
        Date = payload.Date,
        ItemsCount = payload.ItemsCount,
        Total = payload.Total,
        DeliveryDetails = payload.DeliveryDetails,
        Status = payload.Status
      };

      _orders.Insert(0, order);
      return CloneOrder(order);
    }
  }

  public Order? UpdateOrderStatus(int id, OrderStatus status)
  {
    lock (_sync)
    {
      var order = _orders.FirstOrDefault(item => item.Id == id);
      if (order is null)
      {
        return null;
      }

      order.Status = status;
      return CloneOrder(order);
    }
  }

  public bool DeleteOrder(int id)
  {
    lock (_sync)
    {
      var removed = _orders.RemoveAll(item => item.Id == id);
      return removed > 0;
    }
  }

  public List<BlogPost> GetBlogPosts()
  {
    lock (_sync)
    {
      return _blogPosts.Select(CloneBlogPost).ToList();
    }
  }

  public BlogPost? GetBlogPost(int id)
  {
    lock (_sync)
    {
      var post = _blogPosts.FirstOrDefault(item => item.Id == id);
      return post is null ? null : CloneBlogPost(post);
    }
  }

  public BlogPost? GetBlogPostBySlug(string slug)
  {
    lock (_sync)
    {
      var post = _blogPosts.FirstOrDefault(item => item.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
      return post is null ? null : CloneBlogPost(post);
    }
  }

  public BlogPost? GetFeaturedBlogPost()
  {
    lock (_sync)
    {
      var post = _blogPosts.FirstOrDefault(item => item.Featured);
      return post is null ? null : CloneBlogPost(post);
    }
  }

  public BlogPost CreateBlogPost(BlogPostPayload payload)
  {
    lock (_sync)
    {
      var nextId = _blogPosts.Count > 0 ? _blogPosts.Max(item => item.Id) + 1 : 1;
      var publishedAt = payload.PublishedAt == default ? DateTime.UtcNow : payload.PublishedAt;
      var post = new BlogPost
      {
        Id = nextId,
        Slug = payload.Slug,
        Title = payload.Title,
        Excerpt = payload.Excerpt,
        ContentHtml = payload.ContentHtml,
        Content = CloneBlogContent(payload.Content),
        CoverImage = payload.CoverImage,
        Category = payload.Category,
        AuthorName = payload.AuthorName,
        AuthorAvatar = payload.AuthorAvatar,
        AuthorBio = payload.AuthorBio,
        PublishedAt = publishedAt,
        ReadTime = payload.ReadTime,
        Tags = payload.Tags,
        Featured = payload.Featured,
        CoverImageCaption = payload.CoverImageCaption
      };

      _blogPosts.Insert(0, post);
      return CloneBlogPost(post);
    }
  }

  public BlogPost? UpdateBlogPost(int id, BlogPostPayload payload)
  {
    lock (_sync)
    {
      var index = _blogPosts.FindIndex(item => item.Id == id);
      if (index < 0)
      {
        return null;
      }

      var publishedAt = payload.PublishedAt == default ? _blogPosts[index].PublishedAt : payload.PublishedAt;
      var post = new BlogPost
      {
        Id = id,
        Slug = payload.Slug,
        Title = payload.Title,
        Excerpt = payload.Excerpt,
        ContentHtml = payload.ContentHtml,
        Content = CloneBlogContent(payload.Content),
        CoverImage = payload.CoverImage,
        Category = payload.Category,
        AuthorName = payload.AuthorName,
        AuthorAvatar = payload.AuthorAvatar,
        AuthorBio = payload.AuthorBio,
        PublishedAt = publishedAt,
        ReadTime = payload.ReadTime,
        Tags = payload.Tags,
        Featured = payload.Featured,
        CoverImageCaption = payload.CoverImageCaption
      };

      _blogPosts[index] = post;
      return CloneBlogPost(post);
    }
  }

  public bool DeleteBlogPost(int id)
  {
    lock (_sync)
    {
      var removed = _blogPosts.RemoveAll(item => item.Id == id);
      return removed > 0;
    }
  }

  public (List<BlogPost> Items, int Total) FilterBlogPosts(string? searchTerm, string category, int page, int pageSize)
  {
    var filtered = FilterBlogPosts(searchTerm, category);
    var total = filtered.Count;
    var startIndex = (page - 1) * pageSize;
    var items = filtered.Skip(startIndex).Take(pageSize).ToList();
    return (items, total);
  }

  public List<BlogPost> FilterBlogPosts(string? searchTerm, string category)
  {
    var normalizedSearch = searchTerm?.Trim().ToLowerInvariant() ?? string.Empty;
    var posts = GetBlogPosts();
    return posts.Where(post =>
    {
      var matchesCategory = string.IsNullOrWhiteSpace(category) || category == "All" || post.Category == category;
      if (!matchesCategory)
      {
        return false;
      }

      if (string.IsNullOrWhiteSpace(normalizedSearch))
      {
        return true;
      }

      var tagMatch = post.Tags.Any(tag => tag.ToLowerInvariant().Contains(normalizedSearch));
      return post.Title.ToLowerInvariant().Contains(normalizedSearch)
        || post.Excerpt.ToLowerInvariant().Contains(normalizedSearch)
        || post.Category.ToLowerInvariant().Contains(normalizedSearch)
        || post.AuthorName.ToLowerInvariant().Contains(normalizedSearch)
        || tagMatch;
    }).OrderByDescending(post => post.PublishedAt).ToList();
  }

  public List<BlogPost> GetRelatedBlogPosts(string slug, int limit)
  {
    var current = GetBlogPostBySlug(slug);
    if (current is null)
    {
      return [];
    }

    return GetBlogPosts()
      .Where(post => post.Slug != slug && post.Category == current.Category)
      .OrderByDescending(post => post.PublishedAt)
      .Take(limit)
      .ToList();
  }

  public AdminSettings GetSettings()
  {
    lock (_sync)
    {
      return CloneSettings(_settings);
    }
  }

  public AdminSettings SaveSettings(AdminSettings payload)
  {
    lock (_sync)
    {
      _settings = CloneSettings(payload);
      return CloneSettings(_settings);
    }
  }

  public ShippingZone CreateShippingZone(ShippingZone payload)
  {
    lock (_sync)
    {
      var nextId = _settings.ShippingZones.Count > 0
        ? _settings.ShippingZones.Max(zone => zone.Id) + 1
        : 1;
      var zone = new ShippingZone
      {
        Id = nextId,
        Name = payload.Name,
        Region = payload.Region,
        Rates = payload.Rates
      };
      _settings.ShippingZones.Add(zone);
      return CloneShippingZone(zone);
    }
  }

  public ShippingZone? UpdateShippingZone(int id, ShippingZone payload)
  {
    lock (_sync)
    {
      var zone = _settings.ShippingZones.FirstOrDefault(item => item.Id == id);
      if (zone is null)
      {
        return null;
      }

      zone.Name = payload.Name;
      zone.Region = payload.Region;
      zone.Rates = payload.Rates;
      return CloneShippingZone(zone);
    }
  }

  public bool DeleteShippingZone(int id)
  {
    lock (_sync)
    {
      var removed = _settings.ShippingZones.RemoveAll(zone => zone.Id == id);
      return removed > 0;
    }
  }

  public List<Order> FilterOrders(string? searchTerm, string status, string dateRange)
  {
    var orders = GetOrders();
    var normalizedSearch = searchTerm?.Trim().ToLowerInvariant() ?? string.Empty;
    var now = DateTime.UtcNow.Date;

    return orders.Where(order =>
    {
      var matchesSearch = string.IsNullOrWhiteSpace(normalizedSearch)
        || order.OrderId.ToLowerInvariant().Contains(normalizedSearch)
        || order.CustomerName.ToLowerInvariant().Contains(normalizedSearch);

      var matchesStatus = status == "All" || order.Status.ToString() == status;

      var orderDate = DateTime.ParseExact(order.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
      var matchesDate = dateRange switch
      {
        "Last 7 Days" => orderDate >= now.AddDays(-7),
        "Last 30 Days" => orderDate >= now.AddDays(-30),
        "This Year" => orderDate >= new DateTime(now.Year, 1, 1),
        _ => true
      };

      return matchesSearch && matchesStatus && matchesDate;
    }).ToList();
  }

  public (List<Product> Items, int Total) FilterProducts(string? searchTerm, string category, string statusTab, int page, int pageSize)
  {
    var filtered = FilterProducts(searchTerm, category, statusTab);
    var total = filtered.Count;
    var startIndex = (page - 1) * pageSize;
    var items = filtered.Skip(startIndex).Take(pageSize).ToList();
    return (items, total);
  }

  public List<Product> FilterProducts(string? searchTerm, string category, string statusTab)
  {
    var normalizedSearch = searchTerm?.Trim().ToLowerInvariant() ?? string.Empty;
    return GetProducts().Where(product =>
    {
      var matchesSearch = string.IsNullOrWhiteSpace(normalizedSearch)
        || string.Join(' ', new[] { product.Name, product.Sku }.Concat(product.Tags)).ToLowerInvariant().Contains(normalizedSearch);
      var matchesCategory = category == "All Categories" || product.Category == category;
      var matchesStatus = statusTab switch
      {
        "Active" => product.Status == "Active",
        "Drafts" => product.Status == "Draft",
        "Archived" => product.Status == "Archived",
        _ => true
      };
      return matchesSearch && matchesCategory && matchesStatus;
    }).ToList();
  }

  public DashboardStats GetDashboardStats()
  {
    var orders = GetOrders();
    var products = GetProducts();
    var totalRevenue = orders.Sum(order => order.Total);
    var totalOrders = orders.Count;
    var deliveredOrders = orders.Count(order => order.Status == OrderStatus.Delivered);
    var ordersLeft = orders.Count(order => order.Status is OrderStatus.Processing or OrderStatus.Shipped);
    var returnedOrders = orders.Count(order => order.Status == OrderStatus.Refund);
    var customerQueries = Math.Max(3, orders.Select(order => order.CustomerName).Distinct().Count() / 2);
    var totalItems = orders.Sum(order => order.ItemsCount);
    var averageSellingPrice = totalItems > 0 ? totalRevenue / totalItems : 0m;
    var averagePurchaseRate = products.Count > 0 ? products.Average(product => product.PurchaseRate) : 0m;
    var totalPurchaseCost = averagePurchaseRate * totalItems;
    var returnValue = orders.Where(order => order.Status == OrderStatus.Refund).Sum(order => order.Total);
    var returnRate = totalRevenue > 0m
      ? $"{Math.Round((returnValue / totalRevenue) * 100, 1).ToString(CultureInfo.InvariantCulture)}%"
      : "0%";

    return new DashboardStats(
      TotalRevenue: totalRevenue,
      TotalOrders: totalOrders.ToString(CultureInfo.InvariantCulture),
      DeliveredOrders: deliveredOrders.ToString(CultureInfo.InvariantCulture),
      OrdersLeft: ordersLeft.ToString(CultureInfo.InvariantCulture),
      ReturnedOrders: returnedOrders.ToString(CultureInfo.InvariantCulture),
      CustomerQueries: customerQueries.ToString(CultureInfo.InvariantCulture),
      TotalPurchaseCost: totalPurchaseCost,
      AverageSellingPrice: averageSellingPrice,
      ReturnValue: returnValue,
      ReturnRate: returnRate
    );
  }

  public List<OrderItem> GetRecentOrders()
  {
    return GetOrders()
      .OrderByDescending(order => order.Date)
      .Take(5)
      .Select(order => new OrderItem(
        Id: order.OrderId,
        CustomerName: order.CustomerName,
        Date: order.Date,
        Amount: order.Total,
        Status: order.Status switch
        {
          OrderStatus.Processing => "Pending",
          OrderStatus.Shipped => "Shipped",
          _ => "Completed"
        }
      ))
      .ToList();
  }

  public List<PopularProduct> GetPopularProducts()
  {
    return GetProducts()
      .Where(product => product.Featured || product.NewArrival)
      .Take(4)
      .Select((product, index) => new PopularProduct(
        Id: product.Id.ToString(CultureInfo.InvariantCulture),
        Name: product.Name,
        SoldCount: $"{120 + index * 15} sold",
        Price: product.Price,
        ImageUrl: product.ImageUrl ?? product.Images.MainImage.Url
      ))
      .ToList();
  }

  public static List<CategoryNode> BuildCategoryTree(List<Category> categories)
  {
    var grouped = new Dictionary<string?, List<Category>>();
    foreach (var category in categories)
    {
      var key = category.ParentId;
      if (!grouped.TryGetValue(key, out var list))
      {
        list = [];
        grouped[key] = list;
      }
      list.Add(category);
    }

    List<CategoryNode> BuildNodes(string? parentId)
    {
      var items = grouped.TryGetValue(parentId, out var list) ? list : [];
      return items
        .OrderBy(item => item.SortOrder)
        .Select(item => new CategoryNode(item, BuildNodes(item.Id)))
        .ToList();
    }

    return BuildNodes(null);
  }

  private static Category CloneCategory(Category category)
  {
    return new Category
    {
      Id = category.Id,
      Name = category.Name,
      Slug = category.Slug,
      ParentId = category.ParentId,
      Description = category.Description,
      ImageUrl = category.ImageUrl,
      IsVisible = category.IsVisible,
      ProductCount = category.ProductCount,
      SortOrder = category.SortOrder
    };
  }

  private static Product CloneProduct(Product product)
  {
    return new Product
    {
      Id = product.Id,
      Name = product.Name,
      Description = product.Description,
      Category = product.Category,
      SubCategory = product.SubCategory,
      Tags = [.. product.Tags],
      Badges = [.. product.Badges],
      Price = product.Price,
      SalePrice = product.SalePrice,
      PurchaseRate = product.PurchaseRate,
      Gender = product.Gender,
      Ratings = product.Ratings,
      Images = product.Images,
      Variants = product.Variants,
      Meta = product.Meta,
      RelatedProducts = [.. product.RelatedProducts],
      Featured = product.Featured,
      NewArrival = product.NewArrival,
      Sku = product.Sku,
      Stock = product.Stock,
      Status = product.Status,
      ImageUrl = product.ImageUrl,
      StatusActive = product.StatusActive,
      MediaUrls = [.. product.MediaUrls],
      BasePrice = product.BasePrice,
      InventoryVariants = [.. product.InventoryVariants]
    };
  }

  private static Order CloneOrder(Order order)
  {
    return new Order
    {
      Id = order.Id,
      OrderId = order.OrderId,
      CustomerName = order.CustomerName,
      CustomerInitials = order.CustomerInitials,
      Date = order.Date,
      ItemsCount = order.ItemsCount,
      Total = order.Total,
      DeliveryDetails = order.DeliveryDetails,
      Status = order.Status
    };
  }

  private static BlogPost CloneBlogPost(BlogPost post)
  {
    return new BlogPost
    {
      Id = post.Id,
      Slug = post.Slug,
      Title = post.Title,
      Excerpt = post.Excerpt,
      ContentHtml = post.ContentHtml,
      Content = CloneBlogContent(post.Content),
      CoverImage = post.CoverImage,
      Category = post.Category,
      AuthorName = post.AuthorName,
      AuthorAvatar = post.AuthorAvatar,
      AuthorBio = post.AuthorBio,
      PublishedAt = post.PublishedAt,
      ReadTime = post.ReadTime,
      Tags = [.. post.Tags],
      Featured = post.Featured,
      CoverImageCaption = post.CoverImageCaption
    };
  }

  private static List<BlogContentBlock> CloneBlogContent(IEnumerable<BlogContentBlock>? blocks)
  {
    if (blocks is null)
    {
      return [];
    }

    return blocks.Select(block => new BlogContentBlock
    {
      Type = block.Type,
      Text = block.Text,
      ProductId = block.ProductId,
      ProductName = block.ProductName,
      ProductDescription = block.ProductDescription,
      ProductImage = block.ProductImage,
      ProductImageAlt = block.ProductImageAlt
    }).ToList();
  }

  private static AdminSettings CloneSettings(AdminSettings settings)
  {
    return new AdminSettings
    {
      StoreName = settings.StoreName,
      SupportEmail = settings.SupportEmail,
      Description = settings.Description,
      StripeEnabled = settings.StripeEnabled,
      PaypalEnabled = settings.PaypalEnabled,
      StripePublishableKey = settings.StripePublishableKey,
      ShippingZones = settings.ShippingZones.Select(CloneShippingZone).ToList()
    };
  }

  private static ShippingZone CloneShippingZone(ShippingZone zone)
  {
    return new ShippingZone
    {
      Id = zone.Id,
      Name = zone.Name,
      Region = zone.Region,
      Rates = [.. zone.Rates]
    };
  }

  private static List<Category> SeedCategories()
  {
    return
    [
      new Category
      {
        Id = "cat-1",
        Name = "Abayas",
        Slug = "abayas",
        ParentId = null,
        Description = "Elegant modest abayas for everyday wear.",
        ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuC9pYfBZcPDiI5ZJ7g9bIgbGImV-Z1wsYdU5hcfBfdEHJUIA0yeF5Q5qQI6mfKh7VdmufIaDi7X23_5EYjDRCi0UCrWC6gbCQ7gNkeI-Epa7SZrHIcx5yyx0wSlEAgaO7zrAXPlRqThlZlECnyTcS74iDBNslyVfA0C63QZsyzh2Gyw1fTMDReYDksXD73-ImXaOfdBYvAJwiVKAssIZxr5QZoMtfn3CdZGQZFpc3Zgb4zmlHn7e7a7a3PecaGoA6ntPBh_8x0wKKA",
        IsVisible = true,
        ProductCount = 154,
        SortOrder = 1
      },
      new Category
      {
        Id = "cat-1-1",
        Name = "Open Front Abayas",
        Slug = "abayas/open-front-abayas",
        ParentId = "cat-1",
        Description = "Lightweight open front styles for layering.",
        ImageUrl = string.Empty,
        IsVisible = true,
        ProductCount = 42,
        SortOrder = 1
      },
      new Category
      {
        Id = "cat-1-2",
        Name = "Butterfly Abayas",
        Slug = "abayas/butterfly-abayas",
        ParentId = "cat-1",
        Description = "Wide sleeve butterfly silhouettes.",
        ImageUrl = string.Empty,
        IsVisible = true,
        ProductCount = 28,
        SortOrder = 2
      },
      new Category
      {
        Id = "cat-2",
        Name = "Hijabs",
        Slug = "hijabs",
        ParentId = null,
        Description = "Daily essentials for every occasion.",
        ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCyh4AvQGNIapihCyaGbGouOjjAgGzopp36rET2CWil9wRmjTTBwhVcPgepNyOPBMi_FjTENRkYr0oi4MRzgMtkQN0TET_r16n9n-FMdHg-BWMffB5CsduKF_f-1_y46Hr5nwmdlbZLLCMZU5tEdUV-Qgc7St7Cpp7f6PmXuWs7EkNvvznp6_M40XENtAP2AHuRw8QU3xBLLn0cQkfm7Z1lkhZNDewkW0Y4aB1joQ8hvqoAZfSOSDY9jja-PRqudFCfybtMw5X46hY",
        IsVisible = true,
        ProductCount = 120,
        SortOrder = 2
      },
      new Category
      {
        Id = "cat-2-1",
        Name = "Silk Hijabs",
        Slug = "hijabs/silk-hijabs",
        ParentId = "cat-2",
        Description = "Luxurious silk blends in signature shades.",
        ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuDsixXrbLTEQulEO2JHUTqx9QjIIBwROpUY33CR-iaWciLR270YgLMYNlZOb3-dfXPtFupK8pZ2ra_Wg_rXgQX2z4gZduwlmHjPi4UgUYfV_iVH0JJcvhkN8-U5iU1bwEM-gpMj87g87EoisJAsZp-9hpsqYB5G83LHQcC82I_kwViyGE3EgB0tflDNfPQpA2rQCA4Oej9YDRwkPKubcuuYlq1N6xFKCaHcR3nl7CC6bF3bKQfBWrWTsxA4NfHzyr8DnWLoN5lbpEM",
        IsVisible = true,
        ProductCount = 45,
        SortOrder = 1
      },
      new Category
      {
        Id = "cat-2-2",
        Name = "Chiffon Hijabs",
        Slug = "hijabs/chiffon-hijabs",
        ParentId = "cat-2",
        Description = "Lightweight chiffon styles for everyday looks.",
        ImageUrl = string.Empty,
        IsVisible = true,
        ProductCount = 32,
        SortOrder = 2
      },
      new Category
      {
        Id = "cat-3",
        Name = "Prayer Sets",
        Slug = "prayer-sets",
        ParentId = null,
        Description = "Comfortable sets for prayer time.",
        ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuBkKNrLH2kccVXQw-LGx9aFn8RTb6MpnSOqiR3KFGem15Vi2MhsXR0vl22JwuYxPh3_nA3P1so4NkrKOT68KL7vcTFzsIB95gtxjLup28ZXWVz1D7WqcAaoOWbDbNE9PwxHcac4XoVAvZkWOjGZ_eXlVHIpYfaC4Cq_iodTJazXx13JueHNZWg5yj-WRvIjg16MEFSJl_dOtFN18b1t2S-WRS5bI_lq9-8sz7J0OiQyr88KNAWC1O3AtxzRDhFA00I3tMJEfCbsX5U",
        IsVisible = true,
        ProductCount = 8,
        SortOrder = 3
      },
      new Category
      {
        Id = "cat-3-1",
        Name = "Travel Prayer Sets",
        Slug = "prayer-sets/travel-sets",
        ParentId = "cat-3",
        Description = "Compact sets for travel and on-the-go.",
        ImageUrl = string.Empty,
        IsVisible = true,
        ProductCount = 5,
        SortOrder = 1
      }
    ];
  }

  private static List<Product> SeedProducts()
  {
    var ratings = BuildDefaultRatings();

    return
    [
      BuildProduct(
        id: 1,
        name: "Luxe Embroidered Abaya",
        description: "Flowing chiffon abaya with embroidered cuffs.",
        category: "Women",
        subCategory: "Occasion Wear",
        price: 199.00m,
        salePrice: 149.00m,
        purchaseRate: 110.00m,
        gender: "women",
        imageUrl: "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?auto=format&fit=crop&w=600&q=80",
        featured: true,
        newArrival: true,
        ratings: ratings
      ),
      BuildProduct(
        id: 2,
        name: "Classic Linen Abaya",
        description: "Breathable linen blend for daily wear.",
        category: "Women",
        subCategory: "Everyday",
        price: 129.00m,
        salePrice: null,
        purchaseRate: 78.00m,
        gender: "women",
        imageUrl: "https://images.unsplash.com/photo-1524504388940-b1c1722653e1?auto=format&fit=crop&w=600&q=80",
        featured: false,
        newArrival: true,
        ratings: ratings
      ),
      BuildProduct(
        id: 3,
        name: "Essential Jersey Hijab",
        description: "Soft stretch jersey hijab in neutral tones.",
        category: "Accessories",
        subCategory: "Hijabs",
        price: 29.00m,
        salePrice: null,
        purchaseRate: 12.00m,
        gender: "accessories",
        imageUrl: "https://images.unsplash.com/photo-1524504388940-b1c1722653e1?auto=format&fit=crop&w=600&q=80",
        featured: true,
        newArrival: false,
        ratings: ratings
      ),
      BuildProduct(
        id: 4,
        name: "Structured Prayer Set",
        description: "Two-piece prayer set with matching pouch.",
        category: "Women",
        subCategory: "Prayer Sets",
        price: 89.00m,
        salePrice: null,
        purchaseRate: 52.00m,
        gender: "women",
        imageUrl: "https://images.unsplash.com/photo-1520975916090-3105956dac38?auto=format&fit=crop&w=600&q=80",
        featured: false,
        newArrival: false,
        ratings: ratings
      )
    ];
  }

  private static Product BuildProduct(
    int id,
    string name,
    string description,
    string category,
    string subCategory,
    decimal price,
    decimal? salePrice,
    decimal purchaseRate,
    string gender,
    string imageUrl,
    bool featured,
    bool newArrival,
    ProductRatings ratings)
  {
    var basePrice = price;
    var currentPrice = salePrice ?? price;
    var images = new ProductImages(
      new ProductImage("image", "Main", imageUrl, $"{name} image"),
      new List<ProductImage>
      {
        new("image", "Gallery 1", imageUrl, $"{name} gallery 1")
      }
    );
    var variants = new ProductVariants(
      new List<VariantColor>
      {
        new("Midnight", "#111827", true)
      },
      new List<VariantSize>
      {
        new("One Size", 12, true)
      }
    );
    var inventoryVariants = new List<ProductVariantEdit>
    {
      new("One Size / Midnight", currentPrice, $"SKU-{id:D5}-1", 12, imageUrl)
    };
    var stock = inventoryVariants.Sum(item => item.Inventory);

    return new Product
    {
      Id = id,
      Name = name,
      Description = description,
      Category = category,
      SubCategory = subCategory,
      Tags = ["modest", "exclusive"],
      Badges = featured ? ["Featured"] : [],
      Price = currentPrice,
      SalePrice = salePrice,
      PurchaseRate = purchaseRate,
      Gender = gender,
      Ratings = ratings,
      Images = images,
      Variants = variants,
      Meta = new ProductMeta("Dry clean only", "Ships in 2-3 business days"),
      RelatedProducts = [],
      Featured = featured,
      NewArrival = newArrival,
      Sku = $"SKU-{id:D5}",
      Stock = stock,
      Status = stock == 0 ? "Out of Stock" : "Active",
      ImageUrl = imageUrl,
      StatusActive = true,
      MediaUrls = [imageUrl],
      BasePrice = basePrice,
      InventoryVariants = inventoryVariants
    };
  }

  private static List<Order> SeedOrders()
  {
    string DaysAgo(int days) => DateTime.UtcNow.Date.AddDays(-days).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    return
    [
      new Order { Id = 1, OrderId = "#ORD-7782", CustomerName = "Ayesha Khan", CustomerInitials = "AK", Date = DaysAgo(1), ItemsCount = 3, Total = 145.00m, DeliveryDetails = "Size: M | Notes: Weekend pickup", Status = OrderStatus.Processing },
      new Order { Id = 2, OrderId = "#ORD-7781", CustomerName = "Fatima Ahmed", CustomerInitials = "FA", Date = DaysAgo(2), ItemsCount = 1, Total = 89.50m, DeliveryDetails = "Size: S", Status = OrderStatus.Shipped },
      new Order { Id = 3, OrderId = "#ORD-7780", CustomerName = "Zainab Malik", CustomerInitials = "ZM", Date = DaysAgo(2), ItemsCount = 5, Total = 210.00m, DeliveryDetails = "Size: XL | Notes: Gift wrap", Status = OrderStatus.Delivered },
      new Order { Id = 4, OrderId = "#ORD-7779", CustomerName = "Omar Farooq", CustomerInitials = "OF", Date = DaysAgo(3), ItemsCount = 1, Total = 55.00m, DeliveryDetails = "Size: L", Status = OrderStatus.Cancelled },
      new Order { Id = 5, OrderId = "#ORD-7778", CustomerName = "Noura Bashir", CustomerInitials = "NB", Date = DaysAgo(4), ItemsCount = 2, Total = 120.00m, DeliveryDetails = "Size: M | Notes: Leave at front desk", Status = OrderStatus.Processing },
      new Order { Id = 6, OrderId = "#ORD-7777", CustomerName = "Maryam Yusuf", CustomerInitials = "MY", Date = DaysAgo(5), ItemsCount = 4, Total = 275.00m, DeliveryDetails = "Size: XS", Status = OrderStatus.Delivered },
      new Order { Id = 7, OrderId = "#ORD-7776", CustomerName = "Hassan Ali", CustomerInitials = "HA", Date = DaysAgo(6), ItemsCount = 2, Total = 98.00m, DeliveryDetails = "Size: L", Status = OrderStatus.Processing },
      new Order { Id = 8, OrderId = "#ORD-7775", CustomerName = "Sara Noor", CustomerInitials = "SN", Date = DaysAgo(7), ItemsCount = 1, Total = 45.00m, DeliveryDetails = "Size: M | Notes: Call on arrival", Status = OrderStatus.Refund },
      new Order { Id = 9, OrderId = "#ORD-7774", CustomerName = "Bilal Aziz", CustomerInitials = "BA", Date = DaysAgo(8), ItemsCount = 6, Total = 320.00m, DeliveryDetails = "Size: XL", Status = OrderStatus.Shipped },
      new Order { Id = 10, OrderId = "#ORD-7773", CustomerName = "Iman Rashid", CustomerInitials = "IR", Date = DaysAgo(9), ItemsCount = 2, Total = 110.00m, DeliveryDetails = "Size: S", Status = OrderStatus.Delivered },
      new Order { Id = 11, OrderId = "#ORD-7772", CustomerName = "Khadija Noor", CustomerInitials = "KN", Date = DaysAgo(10), ItemsCount = 3, Total = 150.00m, DeliveryDetails = "Size: M | Notes: Gate code 221", Status = OrderStatus.Processing },
      new Order { Id = 12, OrderId = "#ORD-7771", CustomerName = "Usman Tariq", CustomerInitials = "UT", Date = DaysAgo(11), ItemsCount = 1, Total = 75.00m, DeliveryDetails = "Size: L", Status = OrderStatus.Cancelled },
      new Order { Id = 13, OrderId = "#ORD-7770", CustomerName = "Rania Saleh", CustomerInitials = "RS", Date = DaysAgo(12), ItemsCount = 4, Total = 210.00m, DeliveryDetails = "Size: M | Notes: Deliver after 5pm", Status = OrderStatus.Shipped },
      new Order { Id = 14, OrderId = "#ORD-7769", CustomerName = "Yasmin Rahim", CustomerInitials = "YR", Date = DaysAgo(13), ItemsCount = 2, Total = 130.00m, DeliveryDetails = "Size: XS", Status = OrderStatus.Processing },
      new Order { Id = 15, OrderId = "#ORD-7768", CustomerName = "Salim Hadi", CustomerInitials = "SH", Date = DaysAgo(14), ItemsCount = 1, Total = 60.00m, DeliveryDetails = "Size: M", Status = OrderStatus.Refund },
      new Order { Id = 16, OrderId = "#ORD-7767", CustomerName = "Lina Qureshi", CustomerInitials = "LQ", Date = DaysAgo(15), ItemsCount = 3, Total = 190.00m, DeliveryDetails = "Size: L", Status = OrderStatus.Delivered },
      new Order { Id = 17, OrderId = "#ORD-7766", CustomerName = "Faris Zahid", CustomerInitials = "FZ", Date = DaysAgo(16), ItemsCount = 2, Total = 95.00m, DeliveryDetails = "Size: S | Notes: Leave with neighbor", Status = OrderStatus.Processing },
      new Order { Id = 18, OrderId = "#ORD-7765", CustomerName = "Hiba Latif", CustomerInitials = "HL", Date = DaysAgo(17), ItemsCount = 5, Total = 260.00m, DeliveryDetails = "Size: XL", Status = OrderStatus.Shipped },
      new Order { Id = 19, OrderId = "#ORD-7764", CustomerName = "Ola Kareem", CustomerInitials = "OK", Date = DaysAgo(18), ItemsCount = 3, Total = 140.00m, DeliveryDetails = "Size: M", Status = OrderStatus.Delivered },
      new Order { Id = 20, OrderId = "#ORD-7763", CustomerName = "Samiya Ali", CustomerInitials = "SA", Date = DaysAgo(19), ItemsCount = 2, Total = 105.00m, DeliveryDetails = "Size: S | Notes: Ring bell twice", Status = OrderStatus.Processing },
      new Order { Id = 21, OrderId = "#ORD-7762", CustomerName = "Musa Ibrahim", CustomerInitials = "MI", Date = DaysAgo(20), ItemsCount = 1, Total = 70.00m, DeliveryDetails = "Size: L", Status = OrderStatus.Cancelled }
    ];
  }

  private static List<BlogPost> SeedBlogPosts()
  {
    var sharedContent = BuildSharedBlogContent();
    var sharedAvatar =
      "https://lh3.googleusercontent.com/aida-public/AB6AXuDOeayVSwQjnUOIlrLGb40bJACSOrgMhmcMNpmK5GLQ9_8PTCyQpj6JO-p_BFQwGzsKfydm1khc3mNBngB2EaGx13ARPVkkPtPjGSHyTp9kQhDmD9iBpOjwIMVZ0Yxi0w2WpZCoUH3mP8CrDXHo7mHA9_mCYcslil4Keoho0cJqWk9EVpuuvgJWpG8s6lHciOuuxhkH4oEauV5wUrvjfQhDCak3PoZFHwb4Kjt_i4KQ3aiHygfyjMIOm5kJoGv8krYpvhDcrain4yc";

    return
    [
      new BlogPost
      {
        Id = 1,
        Slug = "summer-modesty-stay-cool-and-covered",
        Title = "Summer Modesty: How to Stay Cool and Covered",
        Excerpt =
          "Discover the best breathable fabrics and layering techniques to maintain your personal style and comfort during the warmer months without compromising on modesty.",
        CoverImage =
          "https://lh3.googleusercontent.com/aida-public/AB6AXuBNGqRI8mbwuU74Oenw72ga9HiuAdOWjoe7vF_fmh5QaLoT--WI_teRSizdCjbI63MD9uDEQOCRkLIipu2BJ886B6Zzf8EKnQi2N19QQZam5HpwQ-UeRV9cV3mfLiXFPN2KdDp8NKDIdQEFuvkSFe9f-kaK_EmZDQgYqV0C1fjeciXFAn81xmZuOEwoKmEQqRjfelAwAEwiXgEmqNHDZyKJqQQB43kQLw9HXsoj7DbLWUCnzwTyI3NjzQ45CiAJR24rmd_awm8xT4c",
        Category = "Style Guide",
        AuthorName = "Amina Khan",
        AuthorAvatar = sharedAvatar,
        AuthorBio =
          "Fashion enthusiast and content creator at Arza. I love exploring the intersection of modesty and modern trends.",
        PublishedAt = new DateTime(2024, 5, 15, 0, 0, 0, DateTimeKind.Utc),
        ReadTime = "5 min read",
        Tags = ["ModestFashion", "SummerStyle", "Layering"],
        Featured = true,
        CoverImageCaption = "Exploring airy fabrics and breathable layers for summer days.",
        Content = CloneBlogContent(sharedContent)
      },
      new BlogPost
      {
        Id = 2,
        Slug = "sustainable-fabrics-changing-modest-fashion",
        Title = "Sustainable Fabrics Changing Modest Fashion",
        Excerpt = "From Tencel to organic linen, explore how eco-friendly materials are reshaping the industry and your wardrobe choices.",
        CoverImage =
          "https://lh3.googleusercontent.com/aida-public/AB6AXuCoSLuL6Q1Mx7SqAoQSlBSDcTTDWwWLrwMaFjb1mV-9DLVx5tTV4q01CxqcoHLYs69F9w0GBiwJy6v3Ax00hq-QsVB62THLpF85hU3EChyU_wDiMqP0e5UcZdRQK2TP4Ym9rgN4kJJT2fKOqNgg-e_oIOALjl0KATim12CqlehVN38Y54UxiG6yepBD-Za7ueaeEhdNDf4Tj0arhW4pu-oe6k_lFsoWjY0wV6wJcXMBVZFxOP-1Wen8jZ12X5H3UgNGf3ctf4LgT8k",
        Category = "Trends",
        AuthorName = "Sarah Ahmed",
        AuthorAvatar = sharedAvatar,
        AuthorBio = "Sustainability writer focused on ethical fashion and mindful shopping.",
        PublishedAt = new DateTime(2024, 5, 12, 0, 0, 0, DateTimeKind.Utc),
        ReadTime = "6 min read",
        Tags = ["Sustainability", "EcoFriendly", "Wardrobe"],
        Featured = false,
        CoverImageCaption = "A closer look at textured eco-friendly fabrics.",
        Content = CloneBlogContent(sharedContent)
      },
      new BlogPost
      {
        Id = 3,
        Slug = "essential-hijab-styles-modern-workplace",
        Title = "5 Essential Hijab Styles for the Modern Workplace",
        Excerpt = "Look professional and feel confident with these five simple, elegant, and secure hijab wraps designed for the office.",
        CoverImage =
          "https://lh3.googleusercontent.com/aida-public/AB6AXuAMnHp3XwjAOVT5m-aREBcgvLBF2CHpknEpMTI2V3Y4hX8XKGbe70HOQcUQlFeoTcep0c71lzvXPsY0DZe_A8EWvvikfLNd0W6EyfsuJPUIwwM0Y-eHuT5rG-y52_DV8UaGCZhsD4q3r7q5w2MOaKb4RfH7uKfctaviukTu0od2zDPuzQN-IrO3Q2fZcP2UhcbOSrmvXuIAmyHFP42Z2WSieCPdM0wjyPyuxC2Xthaa13BprTsesjj20U75d8GLKyRaxITNBPbE4SY",
        Category = "Style Guide",
        AuthorName = "Fatima Khan",
        AuthorAvatar =
          "https://lh3.googleusercontent.com/aida-public/AB6AXuDV9doAPdmq0Mhoz9MdzLZ55dytifnuvnXIRqcYIp5muaUQ_gv1RglXDBfjIpZVnJbcqkj-OYpTPKReN_C9sDdBYtDLY9vgf8w8fNIpDBKLhxHJ99huwZgxGIZ4aY_q0BAsZUmMxAoosLwrBTWDHLjEq1-ROR47dzaGMfHrHXQj1ZCKIvnU1Ct-wULGCtLkeerhzTJzgfYPYuRR-j_Sxxbg37gI6zGtRmCuJm1gkxd5Ps9MrdJQbGVBvfk0rsWNhj0QO_I9U-woIaQ",
        AuthorBio = "Styling lead specializing in professional modest fashion.",
        PublishedAt = new DateTime(2024, 5, 10, 0, 0, 0, DateTimeKind.Utc),
        ReadTime = "4 min read",
        Tags = ["Hijab", "Workwear", "Style"],
        Featured = false,
        CoverImageCaption = "Elegant office-ready hijab styling.",
        Content = CloneBlogContent(sharedContent)
      },
      new BlogPost
      {
        Id = 4,
        Slug = "morning-routines-productive-day",
        Title = "Morning Routines for a Productive Day",
        Excerpt =
          "Start your day with intention. We break down a Fajr-centered morning routine that boosts productivity and spiritual mindfulness.",
        CoverImage =
          "https://lh3.googleusercontent.com/aida-public/AB6AXuDqTd2i-40zbf897VfSkb0a0gX3QrmijuE7XIht3sd2QupwZ-RlDRkMNht4GfGj8LObQkANOnckcvwiUyF9SXRhTrr0fgiYlMDtcU9MKK14929IyuLEJ2J-2kRkxTSMZc_qsmPSr3Y1KWPsMAIvV0hhLyZowQ6HgSuAIzyt2WjAC8GcK1Uek7OkzazJ7HD9tq1bHj0_UaBwPeOwXwozvgqw4tr_SY2lCk-Rgai_OuS1nYF31CsHmi8ex6MQbNPUKcPGK5xGLF0wXvQ",
        Category = "Lifestyle",
        AuthorName = "Amina Ross",
        AuthorAvatar = sharedAvatar,
        AuthorBio = "Lifestyle editor sharing mindful routines and wellness tips.",
        PublishedAt = new DateTime(2024, 5, 8, 0, 0, 0, DateTimeKind.Utc),
        ReadTime = "5 min read",
        Tags = ["Lifestyle", "Wellness", "Productivity"],
        Featured = false,
        CoverImageCaption = "A peaceful morning ritual with coffee and a good book.",
        Content = CloneBlogContent(sharedContent)
      },
      new BlogPost
      {
        Id = 5,
        Slug = "sneak-peek-upcoming-eid-collection",
        Title = "Sneak Peek: The Upcoming Eid Collection",
        Excerpt =
          "Get an exclusive first look at our upcoming festive range, featuring rich jewel tones and intricate embroidery.",
        CoverImage =
          "https://lh3.googleusercontent.com/aida-public/AB6AXuBp-E33MKU2ZtFYpexHyaFdAggRbYS3AEKiLrFwcKjnydqsFjgtGHONxq5em9I69mdginhZZYSjViKgALOgMTG7M7_598nNyjg9zhOXjfON-Esvr6WmxRxvFCLW6fg7R6XOSJqt9YV2DMqUo5do6Ifo_KJPUbx7WBZX5Dh4KO3KtSG68K_1pwqbBNvnsfh2p8y4FwUwtJBj9vOSI9U5N3V76hUBDbXW2cqHVDMSTLAOJo_6Cmj0iXzKViIiypwB7SHVHO9k9vd60A4",
        Category = "Collections",
        AuthorName = "Team Arza",
        AuthorAvatar =
          "https://lh3.googleusercontent.com/aida-public/AB6AXuDf_ijS-eALQK3gCysFjznX9Vn5re-Qo_sD3kJlBjrJme0eaSAyV17RE_qiGJ5H--vSYfhgf0AIJvTlbYbnDYNy3Nx1QLw56LQbszSR8ZzDlDxF0e1dcz9E4GOJwLX_YjUT-2UA7eTeXBs5PqBEFKE8X_3qhno6R41sDToZIJMwL7QLjwIJMNnQcVEzeDYXUcuJCY1l_YbvMk32st3H-EVwe4NcBhLQqCQcmlhDvfzkic1MMpKodYx1aPQgn0pBglSjEtJoORroWr4",
        AuthorBio = "Our in-house team highlights seasonal collections and launches.",
        PublishedAt = new DateTime(2024, 5, 5, 0, 0, 0, DateTimeKind.Utc),
        ReadTime = "3 min read",
        Tags = ["Collections", "Eid", "NewArrivals"],
        Featured = false,
        CoverImageCaption = "A preview of our jewel-toned Eid collection.",
        Content = CloneBlogContent(sharedContent)
      },
      new BlogPost
      {
        Id = 6,
        Slug = "finding-stillness-in-a-busy-world",
        Title = "Finding Stillness in a Busy World",
        Excerpt = "Reflections on maintaining a spiritual connection amidst the chaos of modern life, work, and social media.",
        CoverImage =
          "https://lh3.googleusercontent.com/aida-public/AB6AXuAieMKud1SR0wT4bOSc-rWnTUBrYj14weDG88yKBHtM8Icnr-ApTnJAgv0wiQ9QQroBBgnb90l3hr8alalKQWbeTU3_pIfOvJbL95or-OrD9MSmnvU_RW7oGszgiFlzRGfyt3UmUeotsQdnl_r8IN4Ux_n0m7SAUnTVZ1OWm611Z4avTpTt5TcIk1KkxRZirj4CBPE2JmBp4XoWRCiTk9mPHEs5b-W_rYcZOcO-SLnfYHfsO_mwY9zkJ1yKFantci6N3-WzqXpCaw8",
        Category = "Faith",
        AuthorName = "Zainab Ali",
        AuthorAvatar =
          "https://lh3.googleusercontent.com/aida-public/AB6AXuDf_ijS-eALQK3gCysFjznX9Vn5re-Qo_sD3kJlBjrJme0eaSAyV17RE_qiGJ5H--vSYfhgf0AIJvTlbYbnDYNy3Nx1QLw56LQbszSR8ZzDlDxF0e1dcz9E4GOJwLX_YjUT-2UA7eTeXBs5PqBEFKE8X_3qhno6R41sDToZIJMwL7QLjwIJMNnQcVEzeDYXUcuJCY1l_YbvMk32st3H-EVwe4NcBhLQqCQcmlhDvfzkic1MMpKodYx1aPQgn0pBglSjEtJoORroWr4",
        AuthorBio = "Faith writer focusing on mindful living and spiritual balance.",
        PublishedAt = new DateTime(2024, 5, 2, 0, 0, 0, DateTimeKind.Utc),
        ReadTime = "4 min read",
        Tags = ["Faith", "Mindfulness", "Spirituality"],
        Featured = false,
        CoverImageCaption = "A quiet moment of reflection and prayer.",
        Content = CloneBlogContent(sharedContent)
      },
      new BlogPost
      {
        Id = 7,
        Slug = "halal-beauty-brands-to-know",
        Title = "Halal Beauty Brands You Need to Know",
        Excerpt = "A curated list of certified Halal makeup and skincare brands that deliver both quality and peace of mind.",
        CoverImage =
          "https://lh3.googleusercontent.com/aida-public/AB6AXuCX8A9R99qs3i_ijEz6NF9o7enZIUL582O6Y2RhcESir7-RTHavkEnQBzoE6RWH5z4iCNaDXfSlJcKl2wEaTPSn5Gwzrja8cs6rMjNy0d1x2Ouy7dJVVyXUhh-R5YkPPR1XctpufUo3T45Z7iWvk_6KNjKINvWaipdId6-ErrSwKcYxqXtLH3T_CoReob0f7eBtvKrpm78uttj6s2TWPr8u3kj5hU-Ob3Y2_QNZDkX6p78L2HQ3_z7DiuEOwgGQwjpFuixenjwv7mc",
        Category = "Beauty",
        AuthorName = "Layla Omar",
        AuthorAvatar =
          "https://lh3.googleusercontent.com/aida-public/AB6AXuDV9doAPdmq0Mhoz9MdzLZ55dytifnuvnXIRqcYIp5muaUQ_gv1RglXDBfjIpZVnJbcqkj-OYpTPKReN_C9sDdBYtDLY9vgf8w8fNIpDBKLhxHJ99huwZgxGIZ4aY_q0BAsZUmMxAoosLwrBTWDHLjEq1-ROR47dzaGMfHrHXQj1ZCKIvnU1Ct-wULGCtLkeerhzTJzgfYPYuRR-j_Sxxbg37gI6zGtRmCuJm1gkxd5Ps9MrdJQbGVBvfk0rsWNhj0QO_I9U-woIaQ",
        AuthorBio = "Beauty editor spotlighting halal-certified favorites.",
        PublishedAt = new DateTime(2024, 4, 29, 0, 0, 0, DateTimeKind.Utc),
        ReadTime = "3 min read",
        Tags = ["Beauty", "Halal", "Skincare"],
        Featured = false,
        CoverImageCaption = "Minimal makeup essentials for everyday wear.",
        Content = CloneBlogContent(sharedContent)
      },
      new BlogPost
      {
        Id = 8,
        Slug = "mastering-minimalist-hijab-drape",
        Title = "Mastering the Minimalist Hijab Drape",
        Excerpt = "A step-by-step guide to a clean, elegant hijab style that works for any occasion.",
        CoverImage =
          "https://lh3.googleusercontent.com/aida-public/AB6AXuAFWjxTIrN9gZyjoaeIdoRsDSvq5PdxE4d-PB16QylnZ4AEq1p2APgbTWRyC-cnmUPX8ssmIoC-U45w5dMmRF9op6peWoEOf5mgwES1WaKLpm3oINF6oyma918cRQd4E3tsbdFWW-0pP2u1LClSxiafwTtEGBv8bE08GpCQAWn29tCcUxsQkX4mWiygcZ2Pghvu_5D8_b_brJVd0_JgroLphsPgVG_bcTGpIrVzjUQyy-5Kwqi4F_d8mSSdRo4odEjFLaMYpNvqmHM",
        Category = "Tutorial",
        AuthorName = "Rania Noor",
        AuthorAvatar =
          "https://lh3.googleusercontent.com/aida-public/AB6AXuDf_ijS-eALQK3gCysFjznX9Vn5re-Qo_sD3kJlBjrJme0eaSAyV17RE_qiGJ5H--vSYfhgf0AIJvTlbYbnDYNy3Nx1QLw56LQbszSR8ZzDlDxF0e1dcz9E4GOJwLX_YjUT-2UA7eTeXBs5PqBEFKE8X_3qhno6R41sDToZIJMwL7QLjwIJMNnQcVEzeDYXUcuJCY1l_YbvMk32st3H-EVwe4NcBhLQqCQcmlhDvfzkic1MMpKodYx1aPQgn0pBglSjEtJoORroWr4",
        AuthorBio = "Tutorial creator specializing in easy, elegant hijab styles.",
        PublishedAt = new DateTime(2024, 4, 20, 0, 0, 0, DateTimeKind.Utc),
        ReadTime = "4 min read",
        Tags = ["Tutorial", "Hijab", "Styling"],
        Featured = false,
        CoverImageCaption = "A minimalist hijab drape for everyday wear.",
        Content = CloneBlogContent(sharedContent)
      },
      new BlogPost
      {
        Id = 9,
        Slug = "neutral-tones-timeless-wardrobe-staple",
        Title = "Neutral Tones: A Timeless Wardrobe Staple",
        Excerpt = "Why soft neutrals remain the foundation of modest fashion and how to style them year-round.",
        CoverImage =
          "https://lh3.googleusercontent.com/aida-public/AB6AXuDJtFfMuJ1xEJhCgT7DYHFctXiZhcWYPrfzgRK6rEOCXkayrMzNClGCgaFq7QPOJBkvKUv3r8ok38In7OAzn8V7M0roHoT1v5cSn20_WSJUa6dMZfPkDp0Df-ZvrcrdQGe0_XMQklJONeobBtR9mUkmFXNyxvSqbzZoGdb2BGquHhJ-yHI8cLj_A0Z0eqSN0DRwLE9M5-6QaIB7Eg1whmi-Qdx0kB4lrU0YCsW_LpqmWACVrJ_ZhnXf1A5IFLqxDSyI9cYeZ29cnT0",
        Category = "Trends",
        AuthorName = "Nadia Yusuf",
        AuthorAvatar = sharedAvatar,
        AuthorBio = "Trend analyst highlighting timeless wardrobe essentials.",
        PublishedAt = new DateTime(2024, 4, 12, 0, 0, 0, DateTimeKind.Utc),
        ReadTime = "5 min read",
        Tags = ["Trends", "Neutrals", "Wardrobe"],
        Featured = false,
        CoverImageCaption = "Neutral tone layering for modern modest outfits.",
        Content = CloneBlogContent(sharedContent)
      },
      new BlogPost
      {
        Id = 10,
        Slug = "why-we-choose-ethical-fabrics",
        Title = "Why We Choose Ethical Fabrics",
        Excerpt = "Behind the scenes of our sourcing process and what ethical fashion means to us.",
        CoverImage =
          "https://lh3.googleusercontent.com/aida-public/AB6AXuCYDCPfHt0sXGJYq8fv99Y3oTwy4NhNw3zx5yZi9JET2AqXpUj0qdM0BjfD2DvDe42o11UqV5nhvmdjFK9asKLeMntY2Ipj73xV9PVo06SfawgydLxhtFmbUGJihuHN14j1tx8HEYYkdkAirzmVqQP6RvCm4hHbplylE_jh307RPN_1CpWhBXqLCiqU5mI31oGxszTTaAoTEF9_ePQ-bqdOOH7FStNPNky863QJpeCs4ClVFHbnSErnf1mkwOfymHxzhDkyDbuCJgw",
        Category = "Sustainability",
        AuthorName = "Huda Rahman",
        AuthorAvatar =
          "https://lh3.googleusercontent.com/aida-public/AB6AXuDV9doAPdmq0Mhoz9MdzLZ55dytifnuvnXIRqcYIp5muaUQ_gv1RglXDBfjIpZVnJbcqkj-OYpTPKReN_C9sDdBYtDLY9vgf8w8fNIpDBKLhxHJ99huwZgxGIZ4aY_q0BAsZUmMxAoosLwrBTWDHLjEq1-ROR47dzaGMfHrHXQj1ZCKIvnU1Ct-wULGCtLkeerhzTJzgfYPYuRR-j_Sxxbg37gI6zGtRmCuJm1gkxd5Ps9MrdJQbGVBvfk0rsWNhj0QO_I9U-woIaQ",
        AuthorBio = "Ethical sourcing manager and sustainability advocate.",
        PublishedAt = new DateTime(2024, 4, 1, 0, 0, 0, DateTimeKind.Utc),
        ReadTime = "6 min read",
        Tags = ["Sustainability", "Ethical", "Fabrics"],
        Featured = false,
        CoverImageCaption = "Sustainable fabrics chosen for mindful wardrobes.",
        Content = CloneBlogContent(sharedContent)
      }
    ];
  }

  private static List<BlogContentBlock> BuildSharedBlogContent()
  {
    return
    [
      new BlogContentBlock
      {
        Type = "paragraph",
        Text =
          "Modest fashion is not just about covering up; it is about expressing your personal style with elegance and grace. In this season's collection, we focus on earthy tones and breathable fabrics that provide both comfort and sophistication. Layering is key, allowing you to transition seamlessly from the cool breeze of autumn mornings to the warmer afternoon sun."
      },
      new BlogContentBlock
      {
        Type = "heading",
        Text = "Why Earthy Tones?"
      },
      new BlogContentBlock
      {
        Type = "paragraph",
        Text =
          "Nature has always been the ultimate inspiration for designers. This year, we're seeing a resurgence of terracotta, olive green, and deep browns. These colors are versatile and can be mixed and matched effortlessly."
      },
      new BlogContentBlock
      {
        Type = "paragraph",
        Text =
          "When styling these tones, consider texture. A silk scarf in a rich copper hue adds a touch of luxury to a simple linen dress. The contrast between matte and shiny fabrics creates visual interest without overwhelming the outfit."
      },
      new BlogContentBlock
      {
        Type = "product",
        ProductId = 14,
        ProductName = "The Golden Hour Scarf",
        ProductDescription =
          "Crafted from 100% organic silk, perfect for adding a pop of warmth to any ensemble.",
        ProductImage =
          "https://lh3.googleusercontent.com/aida-public/AB6AXuDAl2zpnIGb9p96VWD1bzsbyYDCKk2RdD-7TiOd0VR41dpaJDFdt4Hx4NoQR0ikKAWotz7zh9vARE_3iZ_Dt88hEA81iS5dZ_v_ZhYmVRNcmA6KvVQxpAy4NwXHyNE9JKAdPDru13UwSdIKycmxm9cSEyvn9s3y--uxJ7HKwQv40QE9suFhq8OlitAGp8bLPvl1TmIh26oWJ2HMTMBwq8bMT64NVrxNKe4svmaj1vVfWHkorOcbKV4_OVhwei5Gotb_zDO6Yv-XE0Q",
        ProductImageAlt = "Close up of a mustard yellow hijab"
      },
      new BlogContentBlock
      {
        Type = "heading",
        Text = "The Art of Layering"
      },
      new BlogContentBlock
      {
        Type = "paragraph",
        Text =
          "Layering isn't just practical; it's an art form. Start with a lightweight base, like our classic cotton abaya, and add structure with a long-line cardigan or a structured blazer."
      },
      new BlogContentBlock
      {
        Type = "blockquote",
        Text = "\"Fashion is the armor to survive the reality of everyday life. Modesty is the shield that protects your peace.\""
      },
      new BlogContentBlock
      {
        Type = "paragraph",
        Text =
          "Accessories play a crucial role as well. A thin belt can define the waist without compromising modesty, while statement jewelry can elevate a simple look for an evening event. Remember, the goal is harmony. Each piece should complement the others, creating a cohesive silhouette."
      },
      new BlogContentBlock
      {
        Type = "paragraph",
        Text =
          "As we move forward into the colder months, don't be afraid to experiment. Fashion is personal. Use these trends as a guide, but always stay true to what makes you feel confident and beautiful."
      }
    ];
  }

  private static AdminSettings SeedSettings()
  {
    return new AdminSettings
    {
      StoreName = "Arza",
      SupportEmail = "support@arza.com",
      Description = "A modern modest clothing brand dedicated to quality and style.",
      StripeEnabled = true,
      PaypalEnabled = false,
      StripePublishableKey = "pk_live_51M...xYz2",
      ShippingZones =
      [
        new ShippingZone
        {
          Id = 1,
          Name = "Domestic",
          Region = "United States",
          Rates = ["Free Shipping (>$100)", "Standard: $5.00"]
        },
        new ShippingZone
        {
          Id = 2,
          Name = "International",
          Region = "Rest of World",
          Rates = ["Flat Rate: $25.00"]
        }
      ]
    };
  }

  private static ProductRatings BuildDefaultRatings()
  {
    return new ProductRatings(
      4.6m,
      128,
      [
        new RatingBreakdown(5, 70),
        new RatingBreakdown(4, 20),
        new RatingBreakdown(3, 7),
        new RatingBreakdown(2, 2),
        new RatingBreakdown(1, 1)
      ]
    );
  }

  private List<string> BuildMediaUrls(ProductImages media)
  {
    var urls = new List<string>
    {
      _imageStorage.NormalizeImageName(media.MainImage.Url) ?? string.Empty
    };
    urls.AddRange(media.Thumbnails.Select(item => _imageStorage.NormalizeImageName(item.Url) ?? string.Empty));
    return urls.Where(url => !string.IsNullOrWhiteSpace(url)).Distinct().ToList();
  }

  private static List<ProductVariantEdit> BuildInventoryVariants(ProductCreatePayload payload, int productId, string? imageUrl)
  {
    var baseSku = FormatSku(productId);
    var price = payload.SalePrice ?? payload.Price;
    var primaryColor = payload.Variants.Colors.FirstOrDefault()?.Name ?? "Default";
    var sizes = payload.Variants.Sizes.Count > 0
      ? payload.Variants.Sizes
      : new List<VariantSize> { new("One Size", 0, true) };

    return sizes.Select((size, index) => new ProductVariantEdit(
      $"{size.Label} / {primaryColor}",
      price,
      $"{baseSku}-{index + 1}",
      size.Stock,
      imageUrl
    )).ToList();
  }

  private static ProductImages BuildImagesFromMedia(List<string> mediaUrls, string name, ProductImages? existing)
  {
    var media = mediaUrls.Count > 0 ? mediaUrls : new List<string> { existing?.MainImage.Url ?? string.Empty };
    var valid = media.Where(url => !string.IsNullOrWhiteSpace(url)).ToList();
    if (valid.Count == 0)
    {
      valid.Add(string.Empty);
    }

    var mainUrl = valid[0];
    var thumbnails = valid.Count > 1 ? valid.Skip(1).ToList() : new List<string> { mainUrl };

    var mainImage = new ProductImage(
      existing?.MainImage.Type ?? "image",
      existing?.MainImage.Label ?? "Main",
      mainUrl,
      existing?.MainImage.Alt ?? $"{name} image"
    );

    var thumbnailImages = thumbnails.Select((url, index) => new ProductImage(
      "image",
      existing?.Thumbnails.ElementAtOrDefault(index)?.Label ?? $"Gallery {index + 1}",
      url,
      existing?.Thumbnails.ElementAtOrDefault(index)?.Alt ?? $"{name} gallery {index + 1}"
    )).ToList();

    return new ProductImages(mainImage, thumbnailImages);
  }

  private List<string> NormalizeMediaUrls(IEnumerable<string> mediaUrls)
  {
    return mediaUrls
      .Select(url => _imageStorage.NormalizeImageName(url))
      .Where(url => !string.IsNullOrWhiteSpace(url))
      .Distinct()
      .Select(url => url!)
      .ToList();
  }

  private ProductImages NormalizeProductImages(ProductImages media)
  {
    var mainImage = new ProductImage(
      media.MainImage.Type,
      media.MainImage.Label,
      _imageStorage.NormalizeImageName(media.MainImage.Url) ?? string.Empty,
      media.MainImage.Alt
    );
    var thumbnails = media.Thumbnails.Select(item => new ProductImage(
      item.Type,
      item.Label,
      _imageStorage.NormalizeImageName(item.Url) ?? string.Empty,
      item.Alt
    )).ToList();
    return new ProductImages(mainImage, thumbnails);
  }

  private static string ResolveStatus(bool statusActive, int stock)
  {
    if (!statusActive)
    {
      return "Draft";
    }

    return stock == 0 ? "Out of Stock" : "Active";
  }

  private static string FormatSku(int productId)
  {
    return $"SKU-{productId:D5}";
  }
}
