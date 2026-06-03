namespace KupujDomace.Database.Models;

public class Certificate
{
    public Guid Id { get; set; }
    public Guid FarmId { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int Position { get; set; }
}
