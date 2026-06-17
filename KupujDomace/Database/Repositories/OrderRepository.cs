using KupujDomace.Database.Models;
using KupujDomace.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace KupujDomace.Database.Repositories;

public class OrderRepository
{

    private readonly ShopDbContext _db;

    public OrderRepository(ShopDbContext db) => _db = db;

    public Task<Order?> FindByIdAsync(string orderId) =>
        Guid.TryParse(orderId, out var id)
            ? _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id)
            : Task.FromResult<Order?>(null);

    public async Task<Order> CreateAsync(Guid userId, Guid farmId, string farmName, List<OrderItem> items,
        double total, string currency = "EUR", string? notes = null)
    {
        var now = DateTime.UtcNow;
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FarmId = farmId,
            FarmName = farmName,
            Items = items,
            Total = total,
            Currency = currency,
            Status = "pending",
            Notes = notes,
            CreatedAt = now,
            UpdatedAt = now,
        };
        var position = 0;
        foreach (var item in order.Items)
        {
            item.Id = Guid.NewGuid();
            item.OrderId = order.Id;
            item.Position = position++;
        }
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> UpdateStatusAsync(string orderId, string status)
    {
        var order = await FindByIdAsync(orderId);
        if (order == null) return null;
        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return order;
    }

    public async Task<List<Order>> FindByUserAsync(Guid userId, int skip = 0, int limit = 100) =>
        await _db.Orders.Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .Skip(skip).Take(limit).ToListAsync();

    public async Task<List<Order>> FindByFarmAsync(Guid farmId, string? status = null, int skip = 0, int limit = 100)
    {
        var query = _db.Orders.Include(o => o.Items).Where(o => o.FarmId == farmId);
        if (status != null)
            query = query.Where(o => o.Status == status);
        return await query.OrderByDescending(o => o.CreatedAt).Skip(skip).Take(limit).ToListAsync();
    }

    public async Task<List<OrderStatusCount>> GetFarmStatsAsync(Guid farmId) =>
        await _db.Orders
            .Where(o => o.FarmId == farmId)
            .GroupBy(o => o.Status)
            .Select(g => new OrderStatusCount(g.Key, g.Count(), g.Sum(o => o.Total)))
            .ToListAsync();
}
