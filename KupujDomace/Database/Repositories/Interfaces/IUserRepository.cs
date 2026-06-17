using KupujDomace.Database.Models;

namespace KupujDomace.Database.Repositories.Interfaces;

public interface IUserRepository
{
    string HashPassword(string password);
    bool VerifyPassword(string plain, string hashed);
    Task<User?> FindByEmailAsync(string email);
    Task<User?> FindByIdAsync(string userId);
    Task<User> CreateAsync(string email, string name, string password, string? phone = null, string role = "buyer");
    Task<User> CreateOAuthUserAsync(string email, string name, string? picture, string authProvider);
    Task UpdateAsync(User user);
    Task UpgradeToFarmerAsync(User user, Guid farmId);
    Task<List<User>> GetAllAsync(int skip = 0, int limit = 100);
}
