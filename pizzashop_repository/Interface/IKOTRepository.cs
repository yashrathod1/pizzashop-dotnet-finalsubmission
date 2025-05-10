using pizzashop_repository.Models;

namespace pizzashop_repository.Interface;

public interface IKOTRepository
{
    Task<List<Category>> GetCategory();

    Task<List<Order>> GetOrdersWithItemsAsync(int? categoryId, string status);

    Task<Order?> GetOrderCardWithIdAsync(int orderId);

    Task<OrderItemsMapping?> GetOrderItemAsync(int orderId, int itemId);

    Task UpdateOrderItemAsync(OrderItemsMapping orderItem);

    Task<Order?> GetOrderByIdAsync(int orderId);

    Task<List<OrderItemsMapping>> GetOrderItemsByOrderIdAsync(int orderId);

    Task UpdateOrderAsync(Order order);
}
