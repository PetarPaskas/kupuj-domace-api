using KupujDomace.Database.Models;
using KupujDomace.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KupujDomace.Database.Repositories;

public class CategoryService : ICategoryService
{
    private readonly ShopDbContext _db;
    public CategoryService(ShopDbContext db) => _db = db;
    public async Task<Category?> GetByIdAsync(string categoryId)
    {
        return await _db.Categories.FirstOrDefaultAsync(x => x.Id == categoryId);
    }
}
