using KupujDomace.Database.Models;

namespace KupujDomace.Database.Repositories.Interfaces;

public interface IProductRepository
{
    Task<Product?> FindByIdAsync(string productId);
    Task<Product> CreateAsync(Guid farmId, string name, string description, string categoryId,
        double price, string priceLevel, string photoUrl, string? subcategoryId, string? awardId);
    Task UpdateAsync(Product product);
    Task<bool> DeleteAsync(Product product);
    Task<List<Product>> FindByFarmAsync(Guid farmId, string? categoryId = null);
    Task<List<Product>> FindByCategoryAsync(string categoryId, string? subcategoryId = null, int skip = 0, int limit = 100);
    Task<List<Product>> GetAllAsync(int skip = 0, int limit = 100);
    Task<List<Product>> SearchAsync(string query, int skip = 0, int limit = 100);
    Task<List<string>> GetCategoriesByFarmAsync(Guid farmId);
}
