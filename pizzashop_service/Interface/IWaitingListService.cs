using pizzashop_repository.ViewModels;

namespace pizzashop_service.Interface;

public interface IWaitingListService
{
    Task<WaitingListViewModel> GetSectionAsync();

    Task<List<OrderAppSectionViewModel>> GetAllSectionsAsync();

    Task<WaitingListViewModel> GetWaitingListAsync(int? sectionId);

    Task<bool> AddWaitingTokenInWaitingListAsync(WaitingTokenViewModel waitingTokenVm);

    Task<CustomerViewModel?> GetCustomerByEmail(string email);

    Task<WaitingListItemViewModel?> GetTokenByIdAsync(int id);

    Task<(bool success, string message)> EditWaitingTokenAsync(WaitingTokenViewModel model);

    Task<bool> SoftDeleteTokenAsync(int id);

    Task<List<SectionsViewModal>> GetSectionsWithAvailableTablesAsync();

    Task<AssignTableResultViewModel> AssignTablesToCustomerAsync(AssignTableInWaitingTokenViewModel model);

    Task<List<TableViewModel>> GetAvailableTablesBySectionAsync(int sectionId);
}
