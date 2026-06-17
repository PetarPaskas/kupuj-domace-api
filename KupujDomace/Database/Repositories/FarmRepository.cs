using KupujDomace.Database.Models;
using KupujDomace.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KupujDomace.Database.Repositories;

public class FarmRepository : IFarmRepository
{
    private readonly ShopDbContext _db;

    public FarmRepository(ShopDbContext db) => _db = db;

    private IQueryable<Farm> WithChildren() =>
        _db.Farms.Include(f => f.Photos).Include(f => f.Certificates).Include(f => f.Awards);

    public Task<Farm?> FindByIdAsync(string farmId) =>
        Guid.TryParse(farmId, out var id)
            ? WithChildren().FirstOrDefaultAsync(f => f.Id == id)
            : Task.FromResult<Farm?>(null);

    public Task<Farm?> FindByOwnerAsync(Guid ownerId) =>
        WithChildren().FirstOrDefaultAsync(f => f.OwnerId == ownerId);

    public async Task<Farm> CreateAsync(Guid ownerId, string name, string description, string logoUrl,
        string? location, double? latitude, double? longitude)
    {
        var now = DateTime.UtcNow;
        var farm = new Farm
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            Name = name,
            Description = description,
            LogoUrl = logoUrl,
            Location = location,
            Latitude = latitude,
            Longitude = longitude,
            CreatedAt = now,
            UpdatedAt = now,
        };
        _db.Farms.Add(farm);
        await _db.SaveChangesAsync();
        return farm;
    }

    public async Task UpdateAsync(Farm farm)
    {
        farm.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task AddPhotoAsync(Farm farm, string photoUrl)
    {
        farm.Photos.Add(new FarmPhoto
        {
            Id = Guid.NewGuid(),
            FarmId = farm.Id,
            Url = photoUrl,
            Position = farm.Photos.Count,
        });
        farm.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task RemovePhotoAsync(Farm farm, string photoUrl)
    {
        var toRemove = farm.Photos.Where(p => p.Url == photoUrl).ToList();
        foreach (var photo in toRemove)
        {
            farm.Photos.Remove(photo);
            _db.Remove(photo);
        }
        farm.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task AddCertificateAsync(Farm farm, Certificate certificate)
    {
        certificate.Id = Guid.NewGuid();
        certificate.FarmId = farm.Id;
        certificate.Position = farm.Certificates.Count;
        farm.Certificates.Add(certificate);
        farm.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task AddAwardAsync(Farm farm, Award award)
    {
        award.Id = Guid.NewGuid();
        award.FarmId = farm.Id;
        award.Position = farm.Awards.Count;
        farm.Awards.Add(award);
        farm.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<List<Farm>> GetAllAsync(int skip = 0, int limit = 100) =>
        await WithChildren().Skip(skip).Take(limit).ToListAsync();

    public async Task<List<Farm>> FindByCategoryAsync(string categoryId, string? subcategoryId = null)
    {
        var productQuery = _db.Products.Where(p => p.CategoryId == categoryId && p.IsAvailable);
        if (subcategoryId != null)
            productQuery = productQuery.Where(p => p.SubcategoryId == subcategoryId);

        var farmIds = await productQuery.Select(p => p.FarmId).Distinct().ToListAsync();
        if (farmIds.Count == 0) return new List<Farm>();

        return await WithChildren().Where(f => farmIds.Contains(f.Id)).ToListAsync();
    }

    public async Task<List<Farm>> SearchAsync(string query, int skip = 0, int limit = 100) =>
        await WithChildren()
            .Where(f => EF.Functions.Like(f.Name, $"%{query}%")
                || EF.Functions.Like(f.Description, $"%{query}%")
                || (f.Location != null && EF.Functions.Like(f.Location, $"%{query}%")))
            .Skip(skip).Take(limit).ToListAsync();

    public async Task<bool> DeleteAsync(string farmId)
    {
        if (!Guid.TryParse(farmId, out var id)) return false;
        var farm = await _db.Farms.FirstOrDefaultAsync(f => f.Id == id);
        if (farm == null) return false;
        _db.Farms.Remove(farm);
        await _db.SaveChangesAsync();
        return true;
    }
}
