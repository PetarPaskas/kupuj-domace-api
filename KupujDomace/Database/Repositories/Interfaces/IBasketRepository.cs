using KupujDomace.Database.Models;

namespace KupujDomace.Database.Repositories.Interfaces;

public interface IBasketRepository
{
    Task<Basket?> FindByUserAsync(Guid userId);
    Task<Basket> GetOrCreateAsync(Guid userId);
    Task<Basket> AddItemAsync(Guid userId, Guid productId, Guid farmId, int quantity,
        double price, string priceLevel, string productName, string productPhoto, string farmName);
    Task<Basket?> UpdateItemQuantityAsync(Guid userId, Guid productId, int quantity);
    Task<Basket?> RemoveItemAsync(Guid userId, Guid productId);
    Task<Basket?> ClearAsync(Guid userId);
    Task<Basket?> RemoveFarmItemsAsync(Guid userId, Guid farmId);
}
