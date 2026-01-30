namespace Ecommarce.Domain.Entities;

using Ecommarce.Domain.Common;

public class CartEntity : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public List<CartItemEntity> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
}

public class CartItemEntity : BaseEntity
{
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public ProductEntity? Product { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
