namespace KupujDomace.Database.Models;

public class BasketItem
{
    public Guid Id { get; set; }
    public Guid BasketId { get; set; }
    public Guid ProductId { get; set; }
    public Guid FarmId { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
    public string PriceLevel { get; set; } = "";
    public string ProductName { get; set; } = "";
    public string ProductPhoto { get; set; } = "";
    public string FarmName { get; set; } = "";
    public int Position { get; set; }
}
