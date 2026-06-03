
namespace KupujDomace.Database.Models;

public class Award
{
    public Guid Id { get; set; }
    public Guid FarmId { get; set; }
    public string ClientId { get; set; } = "";   // client-supplied "id", exposed as award.id
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public int? Year { get; set; }
    public string? ImageUrl { get; set; }
    public int Position { get; set; }
}
