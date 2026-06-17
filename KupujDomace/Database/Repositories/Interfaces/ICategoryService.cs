using KupujDomace.Database.Models;

namespace KupujDomace.Database.Repositories.Interfaces
{
    public interface ICategoryService
    {
        Task<Category?> GetByIdAsync(string categoryId);
    }
}
