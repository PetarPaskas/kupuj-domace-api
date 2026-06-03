
namespace KupujDomace.Database.Models;

public class UserSession
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string SessionToken { get; set; } = "";
    public string? EmergentSessionToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
