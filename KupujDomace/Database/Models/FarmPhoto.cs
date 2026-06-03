namespace KupujDomace.Database.Models;

public class FarmPhoto
{
    public Guid Id { get; set; }
    public Guid FarmId { get; set; }
    public string Url { get; set; } = "";
    public int Position { get; set; }
}
