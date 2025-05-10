using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;

namespace pizzashop_service.Interface;

public interface ITableService
{
    // Task<TablesOrderAppViewModel> GetSectionsAsync();

    Task<TablesOrderAppViewModel> GetSectionsWithTablesAsync();

    Task<List<OrderAppSectionViewModel>> GetAllSectionsAsync();

    Task<bool> AddWaitingTokenAsync(WaitingTokenViewModel waitingTokenVm);

    Task<List<WaitingTokenViewModel>> GetWaitingTokens(int sectionId);

    Task<WaitingTokenViewModel> GetCustomerDetailsByToken(int tokenId);

    Task<CustomerViewModel?> GetCustomerByEmail(string email);

    Task<(bool IsSuccess, string Message)> AssignTablesAsync(AssignTableRequestViewModel model);

    Task<int?> GetOrderIdByTableIdAsync(int tableId);

    
}
