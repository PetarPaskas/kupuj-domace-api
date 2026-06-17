using KupujDomace.Database.Models;
using KupujDomace.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace KupujDomace.Database.Repositories;

public class UserSessionRepository : IUserSessionRepository
{
    private readonly ShopDbContext _db;

    public UserSessionRepository(ShopDbContext db) => _db = db;

    public async Task<UserSession> CreateAsync(Guid userId, string sessionToken,
        string? emergentSessionToken, DateTime expiresAt)
    {
        var session = new UserSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SessionToken = sessionToken,
            EmergentSessionToken = emergentSessionToken,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
        };
        _db.UserSessions.Add(session);
        await _db.SaveChangesAsync();
        return session;
    }

    public Task<UserSession?> FindByTokenAsync(string sessionToken) =>
        _db.UserSessions.FirstOrDefaultAsync(s => s.SessionToken == sessionToken);

    public async Task DeleteByTokenAsync(string sessionToken)
    {
        var session = await _db.UserSessions.FirstOrDefaultAsync(s => s.SessionToken == sessionToken);
        if (session != null)
        {
            _db.UserSessions.Remove(session);
            await _db.SaveChangesAsync();
        }
    }
}
