
namespace KupujDomace.Database.Models;

public class Basket
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<BasketItem> Items { get; set; } = new();
}
