using KupujDomace.Database.Models;
using KupujDomace.Database.Repositories.Interfaces;
using KupujDomace.Models;

namespace KupujDomace.Services;


/// <summary>Ports services/product_service.py.</summary>
public class ProductService
{
    private readonly IProductRepository _productRepo;
    private readonly IFarmRepository _farmRepo;
    private readonly ICategoryService _categoryService;

    public ProductService(IProductRepository productRepo, IFarmRepository farmRepo)
    {
        _productRepo = productRepo;
        _farmRepo = farmRepo;
    }

    public async Task<ProductResponse> CreateProductAsync(Guid farmId, Guid ownerId, string name, string description,
        string categoryId, double price, string priceLevel, string photoUrl,
        string? subcategoryId = null, string? awardId = null)
    {
        var farm = await _farmRepo.FindByIdAsync(farmId.ToString());
        if (farm == null)
            throw new HttpError(404, "Farm not found");
        if (farm.OwnerId != ownerId)
            throw new HttpError(403, "Not authorized to add products to this farm");

        if ((await _categoryService.GetByIdAsync(categoryId)) == null)
            throw new HttpError(400, "Invalid category");

        if (!String.IsNullOrEmpty(subcategoryId) && (await _categoryService.GetByIdAsync(subcategoryId) == null))
            throw new HttpError(400, "Invalid subcategory");

        var product = await _productRepo.CreateAsync(farmId, name, Html.StripLinks(description), categoryId.ToString(),
            price, priceLevel, photoUrl, subcategoryId.ToString(), awardId);
        return Mapping.Product(product);
    }

    public async Task<ProductResponse> GetProductAsync(string productId)
    {
        var product = await _productRepo.FindByIdAsync(productId);
        if (product == null)
            throw new HttpError(404, "Product not found");
        return Mapping.Product(product);
    }

    public async Task<ProductResponse> UpdateProductAsync(string productId, Guid ownerId, Doc updateData)
    {
        var product = await _productRepo.FindByIdAsync(productId);
        if (product == null)
            throw new HttpError(404, "Product not found");

        var farm = await _farmRepo.FindByIdAsync(product.FarmId.ToString());
        if (farm == null || farm.OwnerId != ownerId)
            throw new HttpError(403, "Not authorized to update this product");

        if (updateData.TryGetValue("description", out var d) && d is string s && !string.IsNullOrEmpty(s))
            updateData["description"] = Html.StripLinks(s);

        ApplyProductUpdate(product, updateData);
        await _productRepo.UpdateAsync(product);
        return Mapping.Product(product);
    }

    public async Task<bool> DeleteProductAsync(string productId, Guid ownerId)
    {
        var product = await _productRepo.FindByIdAsync(productId);
        if (product == null)
            throw new HttpError(404, "Product not found");

        var farm = await _farmRepo.FindByIdAsync(product.FarmId.ToString());
        if (farm == null || farm.OwnerId != ownerId)
            throw new HttpError(403, "Not authorized to delete this product");

        return await _productRepo.DeleteAsync(product);
    }

    public async Task<List<ProductResponse>> GetProductsByFarmAsync(string farmId, string? categoryId = null)
    {
        if (!Guid.TryParse(farmId, out var id)) return new List<ProductResponse>();
        var products = await _productRepo.FindByFarmAsync(id, categoryId);
        return products.Select(Mapping.Product).ToList();
    }

    public async Task<List<ProductResponse>> GetProductsByCategoryAsync(string categoryId, string? subcategoryId = null,
        int skip = 0, int limit = 100)
    {
        var products = await _productRepo.FindByCategoryAsync(categoryId, subcategoryId, skip, limit);
        return products.Select(Mapping.Product).ToList();
    }

    public async Task<List<ProductResponse>> SearchProductsAsync(string query, int skip = 0, int limit = 100)
    {
        var products = await _productRepo.SearchAsync(query, skip, limit);
        return products.Select(Mapping.Product).ToList();
    }

    public async Task<List<ProductResponse>> GetAllProductsAsync(int skip = 0, int limit = 100)
    {
        var products = await _productRepo.GetAllAsync(skip, limit);
        return products.Select(Mapping.Product).ToList();
    }

    private static void ApplyProductUpdate(Product product, Doc updateData)
    {
        foreach (var (key, value) in updateData)
        {
            switch (key)
            {
                case "name": product.Name = (string)value!; break;
                case "description": product.Description = (string)value!; break;
                case "category_id": product.CategoryId = (string)value!; break;
                case "subcategory_id": product.SubcategoryId = value as string; break;
                case "price": product.Price = Convert.ToDouble(value); break;
                case "price_level": product.PriceLevel = (string)value!; break;
                case "award_id": product.AwardId = value as string; break;
                case "is_available": product.IsAvailable = Convert.ToBoolean(value); break;
            }
        }
    }
}
