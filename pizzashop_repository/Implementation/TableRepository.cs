using Microsoft.EntityFrameworkCore;
using pizzashop_repository.Database;
using pizzashop_repository.Interface;
using pizzashop_repository.Models;


namespace pizzashop_repository.Implementation;

public class TableRepository : ITableRepository
{
    private readonly PizzaShopDbContext _context;

    public TableRepository(PizzaShopDbContext context)
    {
        _context = context;
    }

    public async Task<List<Section>> GetSections()
    {
        return await _context.Sections.Where(c => !c.Isdeleted).ToListAsync();
    }

    public async Task<List<Table>> GetTables()
    {
        return await _context.Tables.Include(t => t.OrdersTableMappings).ThenInclude(ot => ot.Order).ThenInclude(o => o.OrderItemsMappings).Where(c => !c.Isdeleted).ToListAsync();
    }

    public async Task<bool> AddWaitingToken(WaitingToken token)
    {
        await _context.WaitingTokens.AddAsync(token);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddCustomer(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Customer?> GetCustomerByEmailOrMobileAsync(string email, string mobileNo)
    {
        return await _context.Customers
                             .FirstOrDefaultAsync(c => c.Email == email || c.PhoneNumber == mobileNo);
    }
    public async Task<List<WaitingToken>> GetWaitingTokens(int sectionId)
    {
        return await _context.WaitingTokens.Include(w => w.Customer).Where(w => w.SectionId == sectionId && !w.IsAssign).ToListAsync();
    }

    public async Task<WaitingToken?> GetTokenById(int tokenId)
    {
        return await _context.WaitingTokens.Include(w => w.Customer).FirstOrDefaultAsync(w => w.Id == tokenId);
    }

    public async Task<Customer?> GetCustomerByEmail(string email)
    {
        return await _context.Customers.FirstOrDefaultAsync(w => w.Email == email);
    }

    public async Task CreateCustomerAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCustomerAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }

    public async Task CreateRangeAsync(OrdersTableMapping mappings)
    {
        await _context.OrdersTableMappings.AddRangeAsync(mappings);
        await _context.SaveChangesAsync();
    }

    public async Task CreateOrderAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }

    public async Task CreateOrderTaxAsync(OrderTaxesMapping orderTaxes)
    {
        await _context.OrderTaxesMappings.AddAsync(orderTaxes);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Table>> GetTablesByIdsAsync(List<int> tableIds)
    {
        return await _context.Tables
                             .Where(t => tableIds.Contains(t.Id))
                             .ToListAsync();
    }

    public async Task UpdateTableStatusAsync(Table table)
    {
        _context.Tables.Update(table);
        await _context.SaveChangesAsync();
    }

    public async Task<Order?> GetRunningOrderByCustomerIdAsync(int customerId)
    {
        return await _context.Orders
            .FirstOrDefaultAsync(o => o.Customerid == customerId && (o.Status == "In Progress" || o.Status == "Served" || o.Status == "Pending"));
    }

    public async Task<WaitingToken?> GetCustomerFromWaitingList(int customerId)
    {
        return await _context.WaitingTokens
            .FirstOrDefaultAsync(o => o.CustomerId == customerId);
    }

    public async Task<WaitingToken?> GetWaitingTokenByCustomerId(int customerId)
    {
        return await _context.WaitingTokens
            .FirstOrDefaultAsync(wt => wt.CustomerId == customerId && !wt.IsAssign);
    }

    public async Task<int?> GetOrderIdByTableIdAsync(int tableId)
    {
        var mapping = await _context.OrdersTableMappings
                                    .Where(o => o.Tableid == tableId && o.Order.Status != "Completed" &&  o.Order.Status != "Cancelled")
                                    .FirstOrDefaultAsync();
        return mapping?.Orderid;
    }

    public async Task<List<Taxesandfee>> GetTaxesandfeesAsync()
    {
        return await _context.Taxesandfees.Where(t => !t.Isdeleted && t.Isenabled).ToListAsync();
    }

}
