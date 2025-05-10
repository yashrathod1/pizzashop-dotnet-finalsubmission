using pizzashop_repository.Models;

namespace pizzashop_repository.Interface;

public interface ITableRepository
{
    Task<List<Section>> GetSections();

    Task<List<Table>> GetTables();

    Task<bool> AddWaitingToken(WaitingToken token);

    Task<bool> AddCustomer(Customer customer);

    Task<Customer?> GetCustomerByEmailOrMobileAsync(string email, string mobileNo);

    Task<List<WaitingToken>> GetWaitingTokens(int sectionId);

    Task<WaitingToken?> GetTokenById(int tokenId);

    Task<Customer?> GetCustomerByEmail(string email);

    Task CreateCustomerAsync(Customer customer);

    Task UpdateCustomerAsync(Customer customer);

    Task CreateRangeAsync(OrdersTableMapping mappings);

    Task<List<Table>> GetTablesByIdsAsync(List<int> tableIds);

    Task CreateOrderAsync(Order order);

    Task CreateOrderTaxAsync(OrderTaxesMapping orderTaxes);

    Task UpdateTableStatusAsync(Table table);

    Task<Order?> GetRunningOrderByCustomerIdAsync(int customerId);

    Task<WaitingToken?> GetCustomerFromWaitingList(int customerId);

    Task<WaitingToken?> GetWaitingTokenByCustomerId(int customerId);

    Task<int?> GetOrderIdByTableIdAsync(int tableId);

    Task<List<Taxesandfee>> GetTaxesandfeesAsync();
}
