namespace KupujDomace.Database.Models;

public class Product
{
    public Guid Id { get; set; }
    public Guid FarmId { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string CategoryId { get; set; } = "";
    public string? SubcategoryId { get; set; }
    public double Price { get; set; }
    public string PriceLevel { get; set; } = "";
    public string PhotoUrl { get; set; } = "";
    public string? AwardId { get; set; }
    public bool IsAvailable { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Farm? Farm { get; set; }
}
