using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;

namespace pizzashop_repository.Interface;

public interface IOrdersRepository
{
    Task<PagedResult<OrdersTableViewModel>> GetOrdersAsync(int pageNumber, int pageSize, string sortBy, string sortOrder, string searchTerm = "", string status = "All", string dateRange = "All time", string fromDate = "", string toDate = "");

    List<OrdersTableViewModel> GetOrders(string status, string date, string orderId);

    Task<Order?> GetOrderWithDetailsAsync(int orderId);
}
