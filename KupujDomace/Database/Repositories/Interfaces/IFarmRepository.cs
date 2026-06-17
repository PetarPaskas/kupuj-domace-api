using KupujDomace.Database.Models;

namespace KupujDomace.Database.Repositories.Interfaces;

public interface IFarmRepository
{
    Task<Farm?> FindByIdAsync(string farmId);
    Task<Farm?> FindByOwnerAsync(Guid ownerId);
    Task<Farm> CreateAsync(Guid ownerId, string name, string description, string logoUrl,
        string? location, double? latitude, double? longitude);
    Task UpdateAsync(Farm farm);
    Task AddPhotoAsync(Farm farm, string photoUrl);
    Task RemovePhotoAsync(Farm farm, string photoUrl);
    Task AddCertificateAsync(Farm farm, Certificate certificate);
    Task AddAwardAsync(Farm farm, Award award);
    Task<List<Farm>> GetAllAsync(int skip = 0, int limit = 100);
    Task<List<Farm>> FindByCategoryAsync(string categoryId, string? subcategoryId = null);
    Task<List<Farm>> SearchAsync(string query, int skip = 0, int limit = 100);
    Task<bool> DeleteAsync(string farmId);
}
