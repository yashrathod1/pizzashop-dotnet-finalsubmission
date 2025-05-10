using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;

namespace pizzashop_repository.Interface;

public interface ICustomersRepository
{
    Task<PagedResult<CustomerTableViewModel>> GetCustomersAsync(int pageNumber = 1, int pageSize = 5, string sortBy = "Date", string sortOrder = "desc", string searchTerm = "", string dateRange = "All time", DateTime? customStartDate = null, DateTime? customEndDate = null);

    Task<Customer?> GetCustomerByIdAsync(int id);
    Task<List<CustomerTableViewModel>> GetCustomerAsync(string searchTerm = "",string dateRange = "All time",DateTime? customStartDate = null,DateTime? customEndDate = null);
}