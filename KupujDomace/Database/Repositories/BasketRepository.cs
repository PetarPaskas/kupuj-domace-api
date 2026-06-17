using KupujDomace.Database.Models;
using KupujDomace.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KupujDomace.Database.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly ShopDbContext _db;

    public BasketRepository(ShopDbContext db)
    {
        _db = db;
    }

    public Task<Basket?> FindByUserAsync(Guid userId)
    {
        return _db.Baskets.Include(b => b.Items).FirstOrDefaultAsync(b => b.UserId == userId);
    }

    public async Task<Basket> GetOrCreateAsync(Guid userId)
    {
        var basket = await FindByUserAsync(userId);
        if (basket != null) return basket;

        basket = new Basket { Id = Guid.NewGuid(), UserId = userId, UpdatedAt = DateTime.UtcNow };
        _db.Baskets.Add(basket);
        await _db.SaveChangesAsync();
        return basket;
    }

    public async Task<Basket> AddItemAsync(Guid userId, Guid productId, Guid farmId, int quantity,
        double price, string priceLevel, string productName, string productPhoto, string farmName)
    {
        var basket = await GetOrCreateAsync(userId);

        var existing = basket.Items.FirstOrDefault(i => i.ProductId == productId);
        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            basket.Items.Add(new BasketItem
            {
                Id = Guid.NewGuid(),
                BasketId = basket.Id,
                ProductId = productId,
                FarmId = farmId,
                Quantity = quantity,
                Price = price,
                PriceLevel = priceLevel,
                ProductName = productName,
                ProductPhoto = productPhoto,
                FarmName = farmName,
                Position = basket.Items.Count,
            });
        }
        basket.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return basket;
    }

    public async Task<Basket?> UpdateItemQuantityAsync(Guid userId, Guid productId, int quantity)
    {
        var basket = await FindByUserAsync(userId);
        if (basket == null) return null;

        var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            if (quantity <= 0)
            {
                basket.Items.Remove(item);
                _db.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }
        }
        basket.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return basket;
    }

    public async Task<Basket?> RemoveItemAsync(Guid userId, Guid productId)
    {
        var basket = await FindByUserAsync(userId);
        if (basket == null) return null;

        var toRemove = basket.Items.Where(i => i.ProductId == productId).ToList();
        foreach (var item in toRemove)
        {
            basket.Items.Remove(item);
            _db.Remove(item);
        }
        basket.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return basket;
    }

    public async Task<Basket?> ClearAsync(Guid userId)
    {
        var basket = await FindByUserAsync(userId);
        if (basket == null) return null;

        _db.RemoveRange(basket.Items);
        basket.Items.Clear();
        basket.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return basket;
    }

    public async Task<Basket?> RemoveFarmItemsAsync(Guid userId, Guid farmId)
    {
        var basket = await FindByUserAsync(userId);
        if (basket == null) return null;

        var toRemove = basket.Items.Where(i => i.FarmId == farmId).ToList();
        foreach (var item in toRemove)
        {
            basket.Items.Remove(item);
            _db.Remove(item);
        }
        basket.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return basket;
    }
}
