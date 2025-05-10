using Microsoft.EntityFrameworkCore;
using pizzashop_repository.Database;
using pizzashop_repository.Interface;
using pizzashop_repository.Models;

namespace pizzashop_repository.Implementation;

public class KOTRepository : IKOTRepository
{
    private readonly PizzaShopDbContext _context;

    public KOTRepository(PizzaShopDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetCategory()
    {
        return await _context.Categories.Where(c => !c.Isdeleted).OrderBy(c => c.Id).ToListAsync();
    }

    public async Task<List<Order>> GetOrdersWithItemsAsync(int? categoryId, string status)
    {
        return await _context.Orders.Include(o => o.OrdersTableMappings).ThenInclude(t => t.Table).ThenInclude(t => t.Section)
                                    .Include(o => o.OrderItemsMappings).ThenInclude(oi => oi.OrderItemModifiers)
                                    .Include(o => o.OrderItemsMappings).ThenInclude(oi => oi.Menuitem).Where(o => o.OrderItemsMappings.Any(oi =>
                                    !oi.Isdeleted &&
                                    (categoryId == null || oi.Menuitem.Categoryid == categoryId) &&
                                    ((status == "Ready" && oi.Preparedquantity > 0) ||
                                    (status == "In Progress" && (oi.Quantity - oi.Preparedquantity) > 0)))).ToListAsync();
    }

    public async Task<Order?> GetOrderCardWithIdAsync(int orderId)
    {
        return await _context.Orders
            .Include(o => o.OrderItemsMappings)
                .ThenInclude(i => i.OrderItemModifiers)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<OrderItemsMapping?> GetOrderItemAsync(int orderId, int itemId)
    {
        return await _context.OrderItemsMappings
            .FirstOrDefaultAsync(x => x.Orderid == orderId && x.Id == itemId);
    }

    public async Task UpdateOrderItemAsync(OrderItemsMapping orderItem)
    {
        _context.OrderItemsMappings.Update(orderItem);
        await _context.SaveChangesAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(int orderId)
    {
        return await _context.Orders
        .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<List<OrderItemsMapping>> GetOrderItemsByOrderIdAsync(int orderId)
    {
        return await _context.OrderItemsMappings
        .Where(oi => oi.Orderid == orderId && !oi.Isdeleted)
        .ToListAsync();
    }

    public async Task UpdateOrderAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }
}
