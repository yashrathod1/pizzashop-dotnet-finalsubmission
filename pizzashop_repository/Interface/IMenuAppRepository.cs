using pizzashop_repository.Models;

namespace pizzashop_repository.Interface;

public interface IMenuAppRepository
{
     Task<List<Category>> GetCategoriesAsync();

     IQueryable<MenuItem> GetAllItemsQuery();

     Task<MenuItem?> GetItemById(int Id);

     Task<Order?> GetOrderById(int Id);

     Task<bool> UpdateItemAsync(MenuItem item);

     Task<List<MappingMenuItemWithModifier>> GetModifierInItemCardAsync(int id);

     Task<List<OrdersTableMapping>> GetTableDetailsByOrderIdAsync(int orderId);

     Task<List<Modifier>> GetModifiersByIds(List<int> ids);

     Task<List<Taxesandfee>> GetAllTaxesAsync();

     Task<bool> UpdateOrderAsync(Order order);

     Task<bool> InsertOrderItemAsync(OrderItemsMapping item);

     Task<bool> InsertOrderModifierAsync(OrderItemModifier modifier);

     Task<bool> UpdateOrderTaxAsync(OrderTaxesMapping tax);

     Task<bool> DecreaseItemQuantityAsync(int itemId, int quantity);

     Task<bool> DecreaseModifierQuantityAsync(string modifierName, int quantity);

     Task<List<Table>> GetTablesByOrderIdAsync(int orderId);

     Task<bool> UpdateTableAsync(Table table);

     Task<List<OrderItemsMapping>> GetOrderItemsAsync(int orderId);

     Task<List<OrderItemModifier>> GetOrderModifiersAsync(int orderItemId);

     Task<List<OrderTaxesMapping>> GetOrderTaxesAsync(int orderId);

     Task<Payment?> GetPaymentByOrderIdAsync(int orderId);

     Task<bool> InsertPaymentInfoAsync(Payment payment);

     Task<bool> UpdatePaymentInfoAsync(Payment payment);

     Task<OrderItemsMapping?> GetOrderItemByIdAndOrderIdAsync(int itemId, int orderId);

     Task<bool> UpdateOrderItemAsync(OrderItemsMapping orderItems);

     Task<OrderItemsMapping?> GetOrderItemByItemIdAsync(int orderItemId);

     Task<Order?> GetCustomerDetailsFromOrderId(int orderId);

     Task<Customer?> GetCustomerByIdAsync(int id);

     Task<bool> UpdateCustomerAsync(Customer customer);

     Task<List<OrderItemsMapping>> GetOrderItemListByOrderIdAsync(int orderId);

     Task<bool> AddFeedbackAsync(Feedback feedback);

     Task<bool> AddInvoiceAsync(Invoice invoice);
     
     Task<Order?> GetOrderWithDetailsAsync(int orderId);
}
