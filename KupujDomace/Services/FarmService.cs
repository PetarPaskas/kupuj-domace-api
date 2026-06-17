using KupujDomace.Database.Models;
using KupujDomace.Database.Repositories.Interfaces;
using KupujDomace.Models;

namespace KupujDomace.Services;

/// <summary>Ports services/farm_service.py.</summary>
public class FarmService
{
    private readonly IFarmRepository _farmRepo;
    private readonly IProductRepository _productRepo;
    private readonly IUserRepository _userRepo;

    public FarmService(IFarmRepository farmRepo, IProductRepository productRepo, IUserRepository userRepo)
    {
        _farmRepo = farmRepo;
        _productRepo = productRepo;
        _userRepo = userRepo;
    }

    public async Task<FarmResponse> CreateFarmAsync(Guid ownerId, string name, string description, string logoUrl,
        string? location = null, double? latitude = null, double? longitude = null)
    {
        var existing = await _farmRepo.FindByOwnerAsync(ownerId);
        if (existing != null)
            throw new HttpError(400, "User already has a farm");

        var farm = await _farmRepo.CreateAsync(ownerId, name, Html.StripLinks(description), logoUrl,
            location, latitude, longitude);

        var owner = await _userRepo.FindByIdAsync(ownerId.ToString());
        if (owner != null)
            await _userRepo.UpgradeToFarmerAsync(owner, farm.Id);

        return Mapping.Farm(farm);
    }

    private async Task<FarmDetailResponse> EnrichAsync(Farm farm)
    {
        var products = await _productRepo.FindByFarmAsync(farm.Id);
        var categories = products.Select(p => p.CategoryId).Distinct().ToList();
        return Mapping.FarmDetail(farm, products.Count, categories);
    }

    public async Task<FarmDetailResponse> GetFarmAsync(string farmId)
    {
        var farm = await _farmRepo.FindByIdAsync(farmId);
        if (farm == null)
            throw new HttpError(404, "Farm not found");
        return await EnrichAsync(farm);
    }

    public async Task<FarmDetailResponse?> GetFarmByOwnerAsync(Guid ownerId)
    {
        var farm = await _farmRepo.FindByOwnerAsync(ownerId);
        return farm == null ? null : await EnrichAsync(farm);
    }

    public async Task<FarmResponse> UpdateFarmAsync(string farmId, Guid ownerId, Doc updateData)
    {
        var farm = await EnsureOwnedAsync(farmId, ownerId);

        if (updateData.TryGetValue("description", out var d) && d is string s && !string.IsNullOrEmpty(s))
            updateData["description"] = Html.StripLinks(s);

        ApplyFarmUpdate(farm, updateData);
        await _farmRepo.UpdateAsync(farm);
        return Mapping.Farm(farm);
    }

    public async Task<FarmResponse> AddFarmPhotoAsync(string farmId, Guid ownerId, string photoUrl)
    {
        var farm = await EnsureOwnedAsync(farmId, ownerId);
        await _farmRepo.AddPhotoAsync(farm, photoUrl);
        return Mapping.Farm(farm);
    }

    public async Task<FarmResponse> RemoveFarmPhotoAsync(string farmId, Guid ownerId, string photoUrl)
    {
        var farm = await EnsureOwnedAsync(farmId, ownerId);
        await _farmRepo.RemovePhotoAsync(farm, photoUrl);
        return Mapping.Farm(farm);
    }

    public async Task<FarmResponse> AddCertificateAsync(string farmId, Guid ownerId,
        string name, string? description, string? imageUrl)
    {
        var farm = await EnsureOwnedAsync(farmId, ownerId);
        await _farmRepo.AddCertificateAsync(farm, new Certificate
        {
            Name = name,
            Description = description,
            ImageUrl = imageUrl,
        });
        return Mapping.Farm(farm);
    }

    public async Task<FarmResponse> AddAwardAsync(string farmId, Guid ownerId,
        string clientId, string name, string? description, int? year, string? imageUrl)
    {
        var farm = await EnsureOwnedAsync(farmId, ownerId);
        await _farmRepo.AddAwardAsync(farm, new Award
        {
            ClientId = clientId,
            Name = name,
            Description = description,
            Year = year,
            ImageUrl = imageUrl,
        });
        return Mapping.Farm(farm);
    }

    public async Task<List<FarmDetailResponse>> GetAllFarmsAsync(int skip = 0, int limit = 100) =>
        await EnrichManyAsync(await _farmRepo.GetAllAsync(skip, limit));

    public async Task<List<FarmDetailResponse>> GetFarmsByCategoryAsync(string categoryId, string? subcategoryId = null) =>
        await EnrichManyAsync(await _farmRepo.FindByCategoryAsync(categoryId, subcategoryId));

    public async Task<List<FarmDetailResponse>> SearchFarmsAsync(string query, int skip = 0, int limit = 100) =>
        await EnrichManyAsync(await _farmRepo.SearchAsync(query, skip, limit));

    private async Task<List<FarmDetailResponse>> EnrichManyAsync(List<Farm> farms)
    {
        var result = new List<FarmDetailResponse>(farms.Count);
        foreach (var farm in farms)
            result.Add(await EnrichAsync(farm));
        return result;
    }

    private async Task<Farm> EnsureOwnedAsync(string farmId, Guid ownerId)
    {
        var farm = await _farmRepo.FindByIdAsync(farmId);
        if (farm == null)
            throw new HttpError(404, "Farm not found");
        if (farm.OwnerId != ownerId)
            throw new HttpError(403, "Not authorized to update this farm");
        return farm;
    }

    private static void ApplyFarmUpdate(Farm farm, Doc updateData)
    {
        foreach (var (key, value) in updateData)
        {
            switch (key)
            {
                case "name": farm.Name = (string)value!; break;
                case "description": farm.Description = (string)value!; break;
                case "location": farm.Location = value as string; break;
                case "latitude": farm.Latitude = value == null ? null : Convert.ToDouble(value); break;
                case "longitude": farm.Longitude = value == null ? null : Convert.ToDouble(value); break;
            }
        }
    }
}
