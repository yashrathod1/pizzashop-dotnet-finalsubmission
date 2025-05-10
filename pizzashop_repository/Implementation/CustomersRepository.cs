using Microsoft.EntityFrameworkCore;
using pizzashop_repository.Database;
using pizzashop_repository.Interface;
using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;

namespace pizzashop_repository.Implementation;

public class CustomersRepository : ICustomersRepository
{
    private readonly PizzaShopDbContext _context;

    public CustomersRepository(PizzaShopDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<CustomerTableViewModel>> GetCustomersAsync(
     int pageNumber = 1, int pageSize = 5, string sortBy = "Date", string sortOrder = "desc",
     string searchTerm = "", string dateRange = "All time", DateTime? customStartDate = null, DateTime? customEndDate = null)
    {
        IQueryable<Customer>? query = _context.Customers.Include(c => c.Orders).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(i => i.Name.ToLower().Contains(searchTerm.ToLower()));
        }


        DateTime startDate = DateTime.MinValue;
        DateTime endDate = DateTime.MaxValue;

        switch (dateRange)
        {
            case "Last 7 days":
                startDate = DateTime.Now.Date.AddDays(-7);
                endDate = DateTime.Now.Date.AddDays(1).AddTicks(-1);
                break;
            case "Last 30 days":
                startDate = DateTime.Now.Date.AddDays(-30);
                endDate = DateTime.Now.Date.AddDays(1).AddTicks(-1);
                break;
            case "Current Month":
                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                endDate = startDate.AddMonths(1).AddTicks(-1);
                break;
            case "Custom Date":
                if (customStartDate.HasValue && customEndDate.HasValue)
                {
                    startDate = customStartDate.Value.Date;
                    endDate = customEndDate.Value.Date.AddDays(1).AddTicks(-1);
                }
                break;
        }

        query = query.Where(c => c.Orders.Any(o => o.Createdat >= startDate && o.Createdat <= endDate));


        var customerList = await query
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Email,
                c.PhoneNumber,
                Orders = c.Orders
                    .Where(o => o.Createdat >= startDate && o.Createdat <= endDate)
                    .Select(o => new { o.Createdat.Date })
                    .ToList()
            })
            .ToListAsync();

        List<CustomerTableViewModel>? groupedCustomers = customerList
            .SelectMany(c => c.Orders
                .GroupBy(o => o.Date)
                .Select(g => new CustomerTableViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    PhoNo = c.PhoneNumber,
                    Date = g.Key,
                    TotalOrder = g.Count()
                })
            )
            .ToList();


        groupedCustomers = sortBy switch
        {
            "Name" => sortOrder == "asc"
                ? groupedCustomers.OrderBy(c => c.Name).ToList()
                : groupedCustomers.OrderByDescending(c => c.Name).ToList(),

            "Date" => sortOrder == "asc"
                ? groupedCustomers.OrderBy(c => c.Date).ToList()
                : groupedCustomers.OrderByDescending(c => c.Date).ToList(),

            "TotalOrder" => sortOrder == "asc"
                ? groupedCustomers.OrderBy(c => c.TotalOrder).ToList()
                : groupedCustomers.OrderByDescending(c => c.TotalOrder).ToList(),

            _ => groupedCustomers.OrderBy(c => c.Name).ToList()
        };

        int totalCount = groupedCustomers.Count;
        List<CustomerTableViewModel>? paginatedCustomers = groupedCustomers
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<CustomerTableViewModel>(paginatedCustomers, pageNumber, pageSize, totalCount);
    }

    public async Task<Customer?> GetCustomerByIdAsync(int id)
    {
        return await _context.Customers.Include(c => c.Orders).ThenInclude(o => o.OrderItemsMappings)
                                        .Include(c => c.Orders).ThenInclude(o => o.Payment)
                                        .FirstOrDefaultAsync(c => c.Id == id);
    }



    public async Task<List<CustomerTableViewModel>> GetCustomerAsync(
     string searchTerm = "", string dateRange = "All time",
     DateTime? customStartDate = null, DateTime? customEndDate = null)
    {
        DateTime startDate = DateTime.MinValue;
        DateTime endDate = DateTime.MaxValue;

        switch (dateRange)
        {
            case "Last 7 days":
                startDate = DateTime.Now.Date.AddDays(-7);
                endDate = DateTime.Now.Date.AddDays(1).AddTicks(-1);
                break;
            case "Last 30 days":
                startDate = DateTime.Now.Date.AddDays(-30);
                endDate = DateTime.Now.Date.AddDays(1).AddTicks(-1);
                break;
            case "Current Month":
                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                endDate = startDate.AddMonths(1).AddTicks(-1);
                break;
            case "Custom Date":
                if (customStartDate.HasValue && customEndDate.HasValue)
                {
                    startDate = customStartDate.Value.Date;
                    endDate = customEndDate.Value.Date.AddDays(1).AddTicks(-1);
                }
                break;
        }

        var query = _context.Customers
                            .Include(c => c.Orders)
                                .ThenInclude(o => o.Payment)
                            .SelectMany(c => c.Orders
                                .Where(o => o.Createdat >= startDate && o.Createdat <= endDate),
                                (c, o) => new
                                {
                                    c.Id,
                                    c.Name,
                                    c.Email,
                                    c.PhoneNumber,
                                    OrderDate = o.Createdat.Date
                                });


        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(i => i.Name.ToLower().Contains(searchTerm.ToLower()));
        }

        List<CustomerTableViewModel>? groupedCustomers = await query
            .GroupBy(c => new { c.Id, c.Name, c.Email, c.PhoneNumber, c.OrderDate })
            .Select(g => new CustomerTableViewModel
            {
                Id = g.Key.Id,
                Name = g.Key.Name,
                Email = g.Key.Email,
                PhoNo = g.Key.PhoneNumber,
                Date = g.Key.OrderDate,
                TotalOrder = g.Count(),

            })
            .ToListAsync();

        return groupedCustomers;
    }



}
