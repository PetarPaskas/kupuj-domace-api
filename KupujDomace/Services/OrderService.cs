using KupujDomace.Database.Models;
using KupujDomace.Database.Repositories.Interfaces;
using KupujDomace.Models;

namespace KupujDomace.Services;

public class OrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IBasketRepository _basketRepo;
    private readonly IFarmRepository _farmRepo;

    private static readonly string[] ValidStatuses =
        { "pending", "confirmed", "preparing", "ready", "delivered", "cancelled" };

    public OrderService(IOrderRepository orderRepo, IBasketRepository basketRepo, IFarmRepository farmRepo)
    {
        _orderRepo = orderRepo;
        _basketRepo = basketRepo;
        _farmRepo = farmRepo;
    }

    public async Task<List<OrderResponse>> CreateOrdersFromBasketAsync(Guid userId, string? notes = null, string currency = "EUR")
    {
        var basket = await _basketRepo.FindByUserAsync(userId);
        if (basket == null || basket.Items.Count == 0)
            throw new HttpError(400, "Basket is empty");

        var groups = new Dictionary<Guid, (string FarmName, List<OrderItem> Items, double Total)>();
        var order = new List<Guid>();
        foreach (var item in basket.Items.OrderBy(i => i.Position))
        {
            if (!groups.TryGetValue(item.FarmId, out var group))
            {
                group = (item.FarmName, new List<OrderItem>(), 0);
                order.Add(item.FarmId);
            }
            group.Items.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Price,
                PriceLevel = item.PriceLevel,
                ProductName = item.ProductName,
                ProductPhoto = item.ProductPhoto,
            });
            group.Total += item.Price * item.Quantity;
            groups[item.FarmId] = group;
        }

        var orders = new List<OrderResponse>();
        foreach (var farmId in order)
        {
            var group = groups[farmId];
            var created = await _orderRepo.CreateAsync(userId, farmId, group.FarmName, group.Items,
                group.Total, currency, notes);
            orders.Add(Mapping.Order(created));
        }

        await _basketRepo.ClearAsync(userId);
        return orders;
    }

    public async Task<OrderResponse> GetOrderAsync(string orderId, Guid userId)
    {
        var order = await _orderRepo.FindByIdAsync(orderId);
        if (order == null)
            throw new HttpError(404, "Order not found");

        if (order.UserId != userId)
        {
            var farm = await _farmRepo.FindByIdAsync(order.FarmId.ToString());
            if (farm == null || farm.OwnerId != userId)
                throw new HttpError(403, "Not authorized to view this order");
        }

        return Mapping.Order(order);
    }

    public async Task<List<OrderResponse>> GetUserOrdersAsync(Guid userId, int skip = 0, int limit = 100)
    {
        var orders = await _orderRepo.FindByUserAsync(userId, skip, limit);
        return orders.Select(Mapping.Order).ToList();
    }

    public async Task<List<OrderResponse>> GetFarmOrdersAsync(Guid farmId, Guid ownerId, string? status = null,
        int skip = 0, int limit = 100)
    {
        var farm = await _farmRepo.FindByIdAsync(farmId.ToString());
        if (farm == null)
            throw new HttpError(404, "Farm not found");
        if (farm.OwnerId != ownerId)
            throw new HttpError(403, "Not authorized to view orders for this farm");

        var orders = await _orderRepo.FindByFarmAsync(farmId, status, skip, limit);
        return orders.Select(Mapping.Order).ToList();
    }

    public async Task<OrderResponse> UpdateOrderStatusAsync(string orderId, Guid ownerId, string status)
    {
        var order = await _orderRepo.FindByIdAsync(orderId);
        if (order == null)
            throw new HttpError(404, "Order not found");

        var farm = await _farmRepo.FindByIdAsync(order.FarmId.ToString());
        if (farm == null || farm.OwnerId != ownerId)
            throw new HttpError(403, "Not authorized to update this order");

        if (!ValidStatuses.Contains(status))
            throw new HttpError(400, $"Invalid status. Must be one of: [{string.Join(", ", ValidStatuses.Select(s => $"'{s}'"))}]");

        var updated = await _orderRepo.UpdateStatusAsync(orderId, status);
        return Mapping.Order(updated!);
    }

    public async Task<OrderResponse> CancelOrderAsync(string orderId, Guid userId)
    {
        var order = await _orderRepo.FindByIdAsync(orderId);
        if (order == null)
            throw new HttpError(404, "Order not found");

        if (order.UserId != userId)
            throw new HttpError(403, "Not authorized to cancel this order");

        if (order.Status != "pending")
            throw new HttpError(400, "Can only cancel pending orders");

        var updated = await _orderRepo.UpdateStatusAsync(orderId, "cancelled");
        return Mapping.Order(updated!);
    }

    public async Task<FarmStatsResponse> GetFarmStatsAsync(Guid farmId, Guid ownerId)
    {
        var farm = await _farmRepo.FindByIdAsync(farmId.ToString());
        if (farm == null)
            throw new HttpError(404, "Farm not found");
        if (farm.OwnerId != ownerId)
            throw new HttpError(403, "Not authorized to view stats for this farm");

        var counts = new Dictionary<string, int>();
        var totals = new Dictionary<string, double>();
        foreach (var status in ValidStatuses) { counts[status] = 0; totals[status] = 0; }

        foreach (var row in await _orderRepo.GetFarmStatsAsync(farmId))
        {
            counts[row.Status] = row.Count;
            totals[row.Status] = row.Total;
        }

        return new FarmStatsResponse { Counts = counts, Totals = totals };
    }
}
