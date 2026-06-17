using KupujDomace.Database.Models;
using KupujDomace.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KupujDomace.Database.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ShopDbContext _db;

    public UserRepository(ShopDbContext db) => _db = db;

    public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    public bool VerifyPassword(string plain, string hashed)
    {
        if (string.IsNullOrEmpty(hashed)) return false;
        try { return BCrypt.Net.BCrypt.Verify(plain, hashed); }
        catch { return false; }
    }

    public Task<User?> FindByEmailAsync(string email) =>
        _db.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower());

    public Task<User?> FindByIdAsync(string userId) =>
        Guid.TryParse(userId, out var id)
            ? _db.Users.FirstOrDefaultAsync(u => u.Id == id)
            : Task.FromResult<User?>(null);

    public async Task<User> CreateAsync(string email, string name, string password,
        string? phone = null, string role = "buyer")
    {
        var now = DateTime.UtcNow;
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email.ToLower(),
            Name = name,
            Phone = phone,
            PasswordHash = HashPassword(password),
            Role = role,
            FarmId = null,
            CreatedAt = now,
            UpdatedAt = now,
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<User> CreateOAuthUserAsync(string email, string name, string? picture, string authProvider)
    {
        var now = DateTime.UtcNow;
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email.ToLower(),
            Name = name,
            Phone = null,
            PasswordHash = null,
            Role = "buyer",
            FarmId = null,
            Picture = picture,
            AuthProvider = authProvider,
            CreatedAt = now,
            UpdatedAt = now,
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task UpgradeToFarmerAsync(User user, Guid farmId)
    {
        user.Role = "farmer";
        user.FarmId = farmId;
        await UpdateAsync(user);
    }

    public async Task<List<User>> GetAllAsync(int skip = 0, int limit = 100) =>
        await _db.Users.Skip(skip).Take(limit).ToListAsync();
}
