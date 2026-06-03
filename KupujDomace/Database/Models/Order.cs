
namespace KupujDomace.Database.Models;

public class Order
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid FarmId { get; set; }
    public string FarmName { get; set; } = "";
    public double Total { get; set; }
    public string Currency { get; set; } = "EUR";
    public string Status { get; set; } = "pending";
    public string? Notes { get; set; }
    public string? DiscountCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<OrderItem> Items { get; set; } = new();
}
