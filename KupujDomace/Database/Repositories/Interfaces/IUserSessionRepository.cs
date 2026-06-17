using KupujDomace.Database.Models;

namespace KupujDomace.Database.Repositories.Interfaces;

public interface IUserSessionRepository
{
    Task<UserSession> CreateAsync(Guid userId, string sessionToken, string? emergentSessionToken, DateTime expiresAt);
    Task<UserSession?> FindByTokenAsync(string sessionToken);
    Task DeleteByTokenAsync(string sessionToken);
}
