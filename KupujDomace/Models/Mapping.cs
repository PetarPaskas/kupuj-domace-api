using KupujDomace.Database.Models;

namespace KupujDomace.Models;

/// <summary>Maps EF entities to response view models, preserving the legacy JSON shapes.</summary>
public static class Mapping
{
    public static UserResponse User(User u) => new()
    {
        Id = u.Id.ToString(),
        Email = u.Email,
        Name = u.Name,
        Phone = u.Phone,
        Role = u.Role,
        FarmId = u.FarmId?.ToString(),
        Picture = u.Picture,
        AuthProvider = u.AuthProvider,
        CreatedAt = u.CreatedAt,
        UpdatedAt = u.UpdatedAt,
    };

    private static List<string> Photos(Farm f) =>
        f.Photos.OrderBy(p => p.Position).Select(p => p.Url).ToList();

    private static List<CertificateVm> Certificates(Farm f) =>
        f.Certificates.OrderBy(c => c.Position).Select(c => new CertificateVm
        {
            Name = c.Name,
            Description = c.Description,
            ImageUrl = c.ImageUrl,
        }).ToList();

    private static List<AwardVm> Awards(Farm f) =>
        f.Awards.OrderBy(a => a.Position).Select(a => new AwardVm
        {
            Id = a.ClientId,
            Name = a.Name,
            Description = a.Description,
            Year = a.Year,
            ImageUrl = a.ImageUrl,
        }).ToList();

    public static FarmResponse Farm(Farm f) => new()
    {
        Id = f.Id.ToString(),
        OwnerId = f.OwnerId.ToString(),
        Name = f.Name,
        Description = f.Description,
        LogoUrl = f.LogoUrl,
        Photos = Photos(f),
        Location = f.Location,
        Latitude = f.Latitude,
        Longitude = f.Longitude,
        Certificates = Certificates(f),
        Awards = Awards(f),
        CreatedAt = f.CreatedAt,
        UpdatedAt = f.UpdatedAt,
    };

    public static FarmDetailResponse FarmDetail(Farm f, int productCount, List<string> categories) => new()
    {
        Id = f.Id.ToString(),
        OwnerId = f.OwnerId.ToString(),
        Name = f.Name,
        Description = f.Description,
        LogoUrl = f.LogoUrl,
        Photos = Photos(f),
        Location = f.Location,
        Latitude = f.Latitude,
        Longitude = f.Longitude,
        Certificates = Certificates(f),
        Awards = Awards(f),
        CreatedAt = f.CreatedAt,
        UpdatedAt = f.UpdatedAt,
        ProductCount = productCount,
        Categories = categories,
    };

    public static ProductResponse Product(Product p)
    {
        var category = p.Category;
        string? subName = null, subNameSr = null;
        if (!string.IsNullOrEmpty(p.SubcategoryId))
        {
            var sub = p.SubCategory;
            if (sub != null) { subName = p.SubCategory.CategoryName; subNameSr = "Test"; }
        }

        return new ProductResponse
        {
            Id = p.Id.ToString(),
            FarmId = p.FarmId.ToString(),
            Name = p.Name,
            Description = p.Description,
            CategoryId = p.CategoryId,
            SubcategoryId = p.SubcategoryId,
            Price = p.Price,
            PriceLevel = p.PriceLevel,
            PhotoUrl = p.PhotoUrl,
            AwardId = p.AwardId,
            IsAvailable = p.IsAvailable,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            CategoryName = p.Category?.CategoryName ?? string.Empty,
            CategoryNameSr = p.SubCategory?.CategoryName ?? string.Empty,
            SubcategoryName = subName,
            SubcategoryNameSr = subNameSr,
        };
    }

    public static OrderItemVm OrderItem(OrderItem i) => new()
    {
        ProductId = i.ProductId.ToString(),
        Quantity = i.Quantity,
        Price = i.Price,
        PriceLevel = i.PriceLevel,
        ProductName = i.ProductName,
        ProductPhoto = i.ProductPhoto,
    };

    public static OrderResponse Order(Order o) => new()
    {
        Id = o.Id.ToString(),
        UserId = o.UserId.ToString(),
        FarmId = o.FarmId.ToString(),
        FarmName = o.FarmName,
        Items = o.Items.OrderBy(i => i.Position).Select(OrderItem).ToList(),
        Total = o.Total,
        Currency = o.Currency,
        Status = o.Status,
        Notes = o.Notes,
        CreatedAt = o.CreatedAt,
        UpdatedAt = o.UpdatedAt,
    };

    /// <summary>Groups basket items by farm, mirroring BasketService._format_basket.</summary>
    public static BasketResponse Basket(Basket b)
    {
        var groups = new Dictionary<string, (BasketFarmGroupBuilder builder, int order)>();
        var ordered = new List<string>();

        foreach (var item in b.Items.OrderBy(i => i.Position))
        {
            var farmId = item.FarmId.ToString();
            if (!groups.TryGetValue(farmId, out var entry))
            {
                entry = (new BasketFarmGroupBuilder(farmId, item.FarmName), ordered.Count);
                groups[farmId] = entry;
                ordered.Add(farmId);
            }
            entry.builder.Items.Add(new BasketItemVm
            {
                ProductId = item.ProductId.ToString(),
                FarmId = farmId,
                Quantity = item.Quantity,
                Price = item.Price,
                PriceLevel = item.PriceLevel,
                ProductName = item.ProductName,
                ProductPhoto = item.ProductPhoto,
                FarmName = item.FarmName,
            });
            entry.builder.Subtotal += item.Price * item.Quantity;
        }

        var farms = ordered.Select(k => groups[k].builder.Build()).ToList();
        var total = farms.Sum(f => f.Subtotal);
        var itemCount = b.Items.Sum(i => i.Quantity);

        return new BasketResponse
        {
            Id = b.Id.ToString(),
            UserId = b.UserId.ToString(),
            Farms = farms,
            Total = total,
            ItemCount = itemCount,
            UpdatedAt = b.UpdatedAt,
        };
    }

    private sealed class BasketFarmGroupBuilder
    {
        private readonly string _farmId;
        private readonly string _farmName;
        public List<BasketItemVm> Items { get; } = new();
        public double Subtotal { get; set; }

        public BasketFarmGroupBuilder(string farmId, string farmName)
        {
            _farmId = farmId;
            _farmName = farmName;
        }

        public BasketFarmGroup Build() => new()
        {
            FarmId = _farmId,
            FarmName = _farmName,
            Items = Items,
            Subtotal = Subtotal,
        };
    }
}
