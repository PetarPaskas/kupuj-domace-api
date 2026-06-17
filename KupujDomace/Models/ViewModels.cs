namespace KupujDomace.Models;

// Response view models. Property names are PascalCase and serialized to snake_case by the
// global JSON policy (e.g. OwnerId -> owner_id), reproducing the legacy Python JSON shapes.

public record UserResponse
{
    public string Id { get; init; } = "";
    public string Email { get; init; } = "";
    public string Name { get; init; } = "";
    public string? Phone { get; init; }
    public string Role { get; init; } = "buyer";
    public string? FarmId { get; init; }
    public string? Picture { get; init; }
    public string? AuthProvider { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record CertificateVm
{
    public string Name { get; init; } = "";
    public string? Description { get; init; }
    public string? ImageUrl { get; init; }
}

public record AwardVm
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public string? Description { get; init; }
    public int? Year { get; init; }
    public string? ImageUrl { get; init; }
}

public record FarmResponse
{
    public string Id { get; init; } = "";
    public string OwnerId { get; init; } = "";
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public string LogoUrl { get; init; } = "";
    public List<string> Photos { get; init; } = new();
    public string? Location { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public List<CertificateVm> Certificates { get; init; } = new();
    public List<AwardVm> Awards { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

// Returned by the list/detail endpoints (get_farm, my-farm, list, by-category, search),
// which enrich the farm with product_count and categories.
public record FarmDetailResponse : FarmResponse
{
    public int ProductCount { get; init; }
    public List<string> Categories { get; init; } = new();
}

public record ProductResponse
{
    public string Id { get; init; } = "";
    public string FarmId { get; init; } = "";
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public string CategoryId { get; init; } = "";
    public string? SubcategoryId { get; init; }
    public double Price { get; init; }
    public string PriceLevel { get; init; } = "";
    public string PhotoUrl { get; init; } = "";
    public string? AwardId { get; init; }
    public bool IsAvailable { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public string? CategoryName { get; init; }
    public string? CategoryNameSr { get; init; }
    public string? SubcategoryName { get; init; }
    public string? SubcategoryNameSr { get; init; }
}

public record BasketItemVm
{
    public string ProductId { get; init; } = "";
    public string FarmId { get; init; } = "";
    public int Quantity { get; init; }
    public double Price { get; init; }
    public string PriceLevel { get; init; } = "";
    public string ProductName { get; init; } = "";
    public string ProductPhoto { get; init; } = "";
    public string FarmName { get; init; } = "";
}

public record BasketFarmGroup
{
    public string FarmId { get; init; } = "";
    public string FarmName { get; init; } = "";
    public List<BasketItemVm> Items { get; init; } = new();
    public double Subtotal { get; init; }
}

public record BasketResponse
{
    public string Id { get; init; } = "";
    public string UserId { get; init; } = "";
    public List<BasketFarmGroup> Farms { get; init; } = new();
    public double Total { get; init; }
    public int ItemCount { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record OrderItemVm
{
    public string ProductId { get; init; } = "";
    public int Quantity { get; init; }
    public double Price { get; init; }
    public string PriceLevel { get; init; } = "";
    public string ProductName { get; init; } = "";
    public string ProductPhoto { get; init; } = "";
}

public record OrderResponse
{
    public string Id { get; init; } = "";
    public string UserId { get; init; } = "";
    public string FarmId { get; init; } = "";
    public string FarmName { get; init; } = "";
    public List<OrderItemVm> Items { get; init; } = new();
    public double Total { get; init; }
    public string Currency { get; init; } = "EUR";
    public string Status { get; init; } = "pending";
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record AuthResponse(UserResponse User, string AccessToken, string RefreshToken);

public record AccessTokenResponse(string AccessToken);

public record FarmStatsResponse
{
    public Dictionary<string, int> Counts { get; init; } = new();
    public Dictionary<string, double> Totals { get; init; } = new();
}
