using KupujDomace.Database.Repositories.Interfaces;
using KupujDomace.Models;

namespace KupujDomace.Services;

/// <summary>Ports services/basket_service.py, including the farm-grouped response shape.</summary>
public class BasketService
{
    private readonly IBasketRepository _basketRepo;
    private readonly IProductRepository _productRepo;
    private readonly IFarmRepository _farmRepo;

    public BasketService(IBasketRepository basketRepo, IProductRepository productRepo, IFarmRepository farmRepo)
    {
        _basketRepo = basketRepo;
        _productRepo = productRepo;
        _farmRepo = farmRepo;
    }

    public async Task<BasketResponse> GetBasketAsync(Guid userId)
    {
        var basket = await _basketRepo.GetOrCreateAsync(userId);
        return Mapping.Basket(basket);
    }

    public async Task<BasketResponse> AddToBasketAsync(Guid userId, string productId, int quantity = 1)
    {
        var product = await _productRepo.FindByIdAsync(productId);
        if (product == null)
            throw new HttpError(404, "Product not found");
        if (!product.IsAvailable)
            throw new HttpError(400, "Product is not available");

        var farm = await _farmRepo.FindByIdAsync(product.FarmId.ToString());
        if (farm == null)
            throw new HttpError(404, "Farm not found");

        var basket = await _basketRepo.AddItemAsync(userId, product.Id, product.FarmId, quantity,
            product.Price, product.PriceLevel, product.Name, product.PhotoUrl, farm.Name);
        return Mapping.Basket(basket);
    }

    public async Task<BasketResponse> UpdateQuantityAsync(Guid userId, string productId, int quantity)
    {
        if (quantity < 0)
            throw new HttpError(400, "Quantity cannot be negative");

        Guid.TryParse(productId, out var pid);
        var basket = await _basketRepo.UpdateItemQuantityAsync(userId, pid, quantity);
        if (basket == null)
            throw new HttpError(404, "Basket not found");
        return Mapping.Basket(basket);
    }

    public async Task<BasketResponse> RemoveFromBasketAsync(Guid userId, string productId)
    {
        Guid.TryParse(productId, out var pid);
        var basket = await _basketRepo.RemoveItemAsync(userId, pid);
        if (basket == null)
            throw new HttpError(404, "Basket not found");
        return Mapping.Basket(basket);
    }

    public async Task<BasketResponse> ClearBasketAsync(Guid userId)
    {
        var basket = await _basketRepo.ClearAsync(userId);
        if (basket == null)
            throw new HttpError(404, "Basket not found");
        return Mapping.Basket(basket);
    }
}
