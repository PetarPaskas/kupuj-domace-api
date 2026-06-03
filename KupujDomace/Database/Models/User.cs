namespace KupujDomace.Database.Models;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Phone { get; set; }
    public string? PasswordHash { get; set; }
    public string Role { get; set; } = "buyer";
    public Guid? FarmId { get; set; }
    public string? Picture { get; set; }
    public string? AuthProvider { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
