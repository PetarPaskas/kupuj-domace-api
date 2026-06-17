using KupujDomace.Database.Models;
using KupujDomace.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace KupujDomace.Database.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ShopDbContext _db;

    public ProductRepository(ShopDbContext db) => _db = db;

    public Task<Product?> FindByIdAsync(string productId) =>
        Guid.TryParse(productId, out var id)
            ? _db.Products.FirstOrDefaultAsync(p => p.Id == id)
            : Task.FromResult<Product?>(null);

    public async Task<Product> CreateAsync(Guid farmId, string name, string description, string categoryId,
        double price, string priceLevel, string photoUrl, string? subcategoryId, string? awardId)
    {
        var now = DateTime.UtcNow;
        var product = new Product
        {
            Id = Guid.NewGuid(),
            FarmId = farmId,
            Name = name,
            Description = description,
            CategoryId = categoryId,
            SubcategoryId = subcategoryId,
            Price = price,
            PriceLevel = priceLevel,
            PhotoUrl = photoUrl,
            AwardId = awardId,
            IsAvailable = true,
            CreatedAt = now,
            UpdatedAt = now,
        };
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        product.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(Product product)
    {
        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<Product>> FindByFarmAsync(Guid farmId, string? categoryId = null)
    {
        var query = _db.Products.Where(p => p.FarmId == farmId);
        if (categoryId != null)
            query = query.Where(p => p.CategoryId == categoryId);
        return await query.OrderBy(p => p.CategoryId).ToListAsync();
    }

    public async Task<List<Product>> FindByCategoryAsync(string categoryId, string? subcategoryId = null,
        int skip = 0, int limit = 100)
    {
        var query = _db.Products.Where(p => p.CategoryId == categoryId && p.IsAvailable);
        if (subcategoryId != null)
            query = query.Where(p => p.SubcategoryId == subcategoryId);
        return await query.Skip(skip).Take(limit).ToListAsync();
    }

    public async Task<List<Product>> GetAllAsync(int skip = 0, int limit = 100) =>
        await _db.Products.Where(p => p.IsAvailable).Skip(skip).Take(limit).ToListAsync();

    public async Task<List<Product>> SearchAsync(string query, int skip = 0, int limit = 100) =>
        await _db.Products
            .Where(p => p.IsAvailable
                && (EF.Functions.Like(p.Name, $"%{query}%") || EF.Functions.Like(p.Description, $"%{query}%")))
            .Skip(skip).Take(limit).ToListAsync();

    public async Task<List<string>> GetCategoriesByFarmAsync(Guid farmId) =>
        await _db.Products
            .Where(p => p.FarmId == farmId && p.IsAvailable)
            .Select(p => p.CategoryId)
            .Distinct()
            .ToListAsync();
}

