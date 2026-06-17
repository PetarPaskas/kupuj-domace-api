using KupujDomace.Database.Models;
using KupujDomace.Database.Repositories.Interfaces;
using KupujDomace.Models;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AccessTokenResponse = KupujDomace.Models.AccessTokenResponse;

namespace KupujDomace.Services;

/// <summary>Ports services/auth_service.py: JWT (HS256) auth with cookie/Bearer resolution.</summary>
public class AuthService
{
    private const string JwtAlgorithm = SecurityAlgorithms.HmacSha256;
    private const int AccessTokenExpireMinutes = 60 * 24; // 24 hours
    private const int RefreshTokenExpireDays = 7;

    private readonly IConfiguration _config;
    public IUserRepository UserRepo { get; }

    public AuthService(IUserRepository userRepo, IConfiguration config)
    {
        UserRepo = userRepo;
        _config = config;
    }

    public string GetJwtSecret() =>
        _config["JWT_SECRET"] ?? "your-secret-key-change-in-production";

    private SymmetricSecurityKey SigningKey() => new(Encoding.UTF8.GetBytes(GetJwtSecret()));

    public string CreateAccessToken(string userId, string email, string role)
    {
        var token = new JwtSecurityToken(
            claims: new[]
            {
                new Claim("sub", userId),
                new Claim("email", email),
                new Claim("role", role),
                new Claim("type", "access"),
            },
            expires: DateTime.UtcNow.AddMinutes(AccessTokenExpireMinutes),
            signingCredentials: new SigningCredentials(SigningKey(), JwtAlgorithm));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string CreateRefreshToken(string userId)
    {
        var token = new JwtSecurityToken(
            claims: new[] { new Claim("sub", userId), new Claim("type", "refresh") },
            expires: DateTime.UtcNow.AddDays(RefreshTokenExpireDays),
            signingCredentials: new SigningCredentials(SigningKey(), JwtAlgorithm));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Doc DecodeToken(string token)
    {
        var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = SigningKey(),
            ClockSkew = TimeSpan.Zero,
        };
        try
        {
            handler.ValidateToken(token, parameters, out var validated);
            var jwt = (JwtSecurityToken)validated;
            var payload = new Doc();
            foreach (var kv in jwt.Payload)
                payload[kv.Key] = kv.Value;
            return payload;
        }
        catch (SecurityTokenExpiredException)
        {
            throw new HttpError(401, "Token expired");
        }
        catch (HttpError)
        {
            throw;
        }
        catch
        {
            throw new HttpError(401, "Invalid token");
        }
    }

    public async Task<AuthResponse> RegisterAsync(string email, string name, string password, string? phone = null)
    {
        var existing = await UserRepo.FindByEmailAsync(email);
        if (existing != null)
            throw new HttpError(400, "Email already registered");

        var user = await UserRepo.CreateAsync(email, name, password, phone);
        return new AuthResponse(Mapping.User(user),
            CreateAccessToken(user.Id.ToString(), user.Email, user.Role),
            CreateRefreshToken(user.Id.ToString()));
    }

    public async Task<AuthResponse> LoginAsync(string email, string password)
    {
        var user = await UserRepo.FindByEmailAsync(email);
        if (user == null)
            throw new HttpError(401, "Invalid email or password");

        if (!UserRepo.VerifyPassword(password, user.PasswordHash ?? ""))
            throw new HttpError(401, "Invalid email or password");

        return new AuthResponse(Mapping.User(user),
            CreateAccessToken(user.Id.ToString(), user.Email, user.Role),
            CreateRefreshToken(user.Id.ToString()));
    }

    public async Task<User> GetCurrentUserAsync(HttpContext ctx)
    {
        var token = ctx.Request.Cookies["access_token"];
        if (string.IsNullOrEmpty(token))
        {
            var authHeader = ctx.Request.Headers.Authorization.ToString();
            if (authHeader.StartsWith("Bearer "))
                token = authHeader.Substring(7);
        }

        if (string.IsNullOrEmpty(token))
            throw new HttpError(401, "Not authenticated");

        var payload = DecodeToken(token);
        if (payload.GetValueOrDefault("type") as string != "access")
            throw new HttpError(401, "Invalid token type");

        var user = await UserRepo.FindByIdAsync((string)payload["sub"]!);
        if (user == null)
            throw new HttpError(401, "User not found");

        return user;
    }

    public async Task<AccessTokenResponse> RefreshAccessTokenAsync(string refreshToken)
    {
        var payload = DecodeToken(refreshToken);
        if (payload.GetValueOrDefault("type") as string != "refresh")
            throw new HttpError(401, "Invalid token type");

        var user = await UserRepo.FindByIdAsync((string)payload["sub"]!);
        if (user == null)
            throw new HttpError(401, "User not found");

        return new AccessTokenResponse(CreateAccessToken(user.Id.ToString(), user.Email, user.Role));
    }

    public async Task<User?> GetOptionalUserAsync(HttpContext ctx)
    {
        try { return await GetCurrentUserAsync(ctx); }
        catch (HttpError) { return null; }
    }
}
