using KupujDomace.Database.Models;
using KupujDomace.Models;

namespace KupujDomace.Database.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<Order?> FindByIdAsync(string orderId);
    Task<Order> CreateAsync(Guid userId, Guid farmId, string farmName, List<OrderItem> items,
        double total, string currency = "EUR", string? notes = null);
    Task<Order?> UpdateStatusAsync(string orderId, string status);
    Task<List<Order>> FindByUserAsync(Guid userId, int skip = 0, int limit = 100);
    Task<List<Order>> FindByFarmAsync(Guid farmId, string? status = null, int skip = 0, int limit = 100);
    Task<List<OrderStatusCount>> GetFarmStatsAsync(Guid farmId);
}
