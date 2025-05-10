using pizzashop_repository.Models;

namespace pizzashop_repository.Interface;

public interface IWaitingListRepository
{
    Task<List<Section>> GetSectionAsync();

    Task<List<WaitingToken>> GetWaitingListAsync(int? sectionId);

    Task<bool> AddWaitingToken(WaitingToken token);

    Task<bool> AddCustomer(Customer customer);

    Task<Customer?> GetCustomerByEmail(string email);

    Task<WaitingToken?> GetWaitingTokenByCustomerId(int customerId);

    Task<WaitingToken?> GetWaitingTokenById(int id);

    Task<List<WaitingToken>> GetAllWaitingList();

    Task UpdateAsync(WaitingToken waitingToken);

    Task<List<Section>> GetSectionsWithAvailableTablesAsync();

    Task<List<Table>> GetAvailableTablesBySectionAsync(int sectionId);

    Task<List<Table>> GetTablesByIdsAsync(List<int> tableIds);

    Task<Customer?> GetCustomerByIdAsync(int customerId);

    Task<bool> CreateOrderAsync(Order order);

    Task<bool> CreateRangeAsync(OrdersTableMapping mapping);

    Task<bool> UpdateTableStatusAsync(Table table);

    Task<List<Taxesandfee>> GetTaxesandfeesAsync();

    Task<bool> CreateOrderTaxAsync(OrderTaxesMapping tax);

    Task<WaitingToken?> GetCustomerFromWaitingList(int customerId);

    Task<bool> UpdateWaitingCustomerAsync(WaitingToken waitingToken);
}