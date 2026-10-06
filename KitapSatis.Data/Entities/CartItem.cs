namespace KitapSatis.Data.Entities;

public class CartItem : BaseEntity
{
    public string UserId { get; set; } = string.Empty;

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public int Quantity { get; set; }
}