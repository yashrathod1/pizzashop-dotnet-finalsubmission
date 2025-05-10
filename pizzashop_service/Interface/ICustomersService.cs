using pizzashop_repository.ViewModels;

namespace pizzashop_service.Interface;

public interface ICustomersService
{
    Task<PagedResult<CustomerTableViewModel>> GetCustomersAsync(int pageNumber = 1, int pageSize = 5, string sortBy = "Date", string sortOrder = "desc", string searchTerm = "", string dateRange = "All time", DateTime? customStartDate = null, DateTime? customEndDate = null);

    Task<CustomerHistoryViewModel> GetCustomerHistoryAsync(int id);

    Task<byte[]> GenerateExcel(string searchTerm = "", string dateRange = "All time", DateTime? customStartDate = null, DateTime? customEndDate = null);
}
