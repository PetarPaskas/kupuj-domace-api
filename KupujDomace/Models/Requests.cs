namespace KupujDomace.Models;

// Request DTOs mirror the Pydantic request models defined in the Python routers.
// Property names are PascalCase; the global SnakeCaseLower JSON policy binds the
// snake_case request bodies (e.g. logo_url -> LogoUrl).

// ----- Auth (auth_router.py) -----
public record RegisterRequest(string Email, string Name, string Password, string? Phone = null);
public record LoginRequest(string Email, string Password);
public record RefreshRequest(string? RefreshToken = null);
public record UpdateProfileRequest(string? Name = null, string? Phone = null);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
public record GoogleSessionRequest(string SessionId);

// ----- Farms (farm_router.py) -----
public record FarmCreateRequest(
    string Name,
    string Description,
    string LogoUrl,
    string? Location = null,
    double? Latitude = null,
    double? Longitude = null);

public record CertificateRequest(string Name, string? Description = null, string? ImageUrl = null);
public record AwardRequest(string Id, string Name, string? Description = null, int? Year = null, string? ImageUrl = null);
public record PhotoRequest(string PhotoUrl);

// ----- Products (product_router.py) -----
public record ProductCreateRequest(
    string Name,
    string Description,
    string CategoryId,
    double Price,
    string PriceLevel,
    string PhotoUrl,
    string? SubcategoryId = null,
    string? AwardId = null);

// ----- Basket (basket_router.py) -----
public record AddToBasketRequest(string ProductId, int Quantity = 1);
public record UpdateQuantityRequest(int Quantity);

// ----- Orders (order_router.py) -----
public record CreateOrderRequest(string? Notes = null, string Currency = "EUR");
public record UpdateStatusRequest(string Status);

