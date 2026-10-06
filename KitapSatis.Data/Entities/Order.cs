namespace KitapSatis.Data.Entities;

public class Order : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Hazırlanıyor";

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}