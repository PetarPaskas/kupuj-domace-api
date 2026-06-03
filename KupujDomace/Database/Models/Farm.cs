namespace KupujDomace.Database.Models;

public class Farm
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string LogoUrl { get; set; } = "";
    public string? Location { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<FarmPhoto> Photos { get; set; } = new();
    public List<Certificate> Certificates { get; set; } = new();
    public List<Award> Awards { get; set; } = new();
    public List<Product> Products { get; set; } = new();
}
