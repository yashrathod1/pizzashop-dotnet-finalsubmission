using pizzashop_repository.Interface;
using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;
using pizzashop_service.Interface;

namespace pizzashop_service.Implementation;

public class TableService : ITableService
{
    private readonly ITableRepository _tableRepository;

    public TableService(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public async Task<TablesOrderAppViewModel> GetSectionsWithTablesAsync()
    {
        var sections = await _tableRepository.GetSections();
        var tables = await _tableRepository.GetTables();

        var viewModel = new TablesOrderAppViewModel
        {
            Sections = sections.Select(s => new OrderAppSectionViewModel
            {
                Id = s.Id,
                Name = s.Name,
                AvailableCount = tables.Count(t => t.Sectionid == s.Id && t.Status == "Available"),
                AssignedCount = tables.Count(t => t.Sectionid == s.Id && t.Status == "Assigned"),
                OccupiedCount = tables.Count(t => t.Sectionid == s.Id && t.Status == "Occupied"),
            }).ToList(),

            Tables = tables.Select(t => new OrderAppTableViewModel
            {
                Id = t.Id,
                Name = t.Name,
                SectionId = t.Sectionid,
                Capacity = t.Capacity,
                Status = t.Status,
                OrderTableTime = t.OrdersTableMappings.Where(ot => ot.Order.Status != "Completed" && ot.Order.Status != "Cancelled").Select(ot => ot.Order.Createdat).FirstOrDefault(),
                TotalAmount = t.OrdersTableMappings.Select(ot => ot.Order).Where(o => o.Status == "In Progress" || o.Status == "Served").Select(o => o.Totalamount).FirstOrDefault(),
            }).ToList(),
        };

        return viewModel;
    }

    public async Task<List<OrderAppSectionViewModel>> GetAllSectionsAsync()
    {
        var sections = await _tableRepository.GetSections();

        return sections.Select(s => new OrderAppSectionViewModel
        {
            Id = s.Id,
            Name = s.Name
        }).ToList();
    }

    public async Task<bool> AddWaitingTokenAsync(WaitingTokenViewModel waitingTokenVm)
    {
        var existingCustomer = await _tableRepository.GetCustomerByEmailOrMobileAsync(waitingTokenVm.Email, waitingTokenVm.MobileNo);

        Customer customer;

        if (existingCustomer == null)
        {
            customer = new Customer
            {
                Name = waitingTokenVm.Name,
                Email = waitingTokenVm.Email,
                PhoneNumber = waitingTokenVm.MobileNo
            };

            await _tableRepository.AddCustomer(customer);
        }
        else
        {
            customer = existingCustomer;
            var existingToken = await _tableRepository.GetWaitingTokenByCustomerId(customer.Id);
            if (existingToken != null)
            {
                return false;
            }
        }

        var waitingToken = new WaitingToken
        {
            CustomerId = customer.Id,
            NoOfPersons = waitingTokenVm.NoOfPerson,
            SectionId = waitingTokenVm.SectionId
        };

        var result = await _tableRepository.AddWaitingToken(waitingToken);

        return result;
    }

    public async Task<List<WaitingTokenViewModel>> GetWaitingTokens(int sectionId)
    {
        var tokens = await _tableRepository.GetWaitingTokens(sectionId);
        var waitingTokens = tokens.Select(t => new WaitingTokenViewModel
        {
            Id = t.Id,
            Name = t.Customer.Name,
            NoOfPerson = t.NoOfPersons
        }).ToList();

        return waitingTokens;
    }

    public async Task<WaitingTokenViewModel> GetCustomerDetailsByToken(int tokenId)
    {
        try
        {
            var token = await _tableRepository.GetTokenById(tokenId);
            if (token == null) return null;

            return new WaitingTokenViewModel
            {
                Id = token.Id,
                Email = token.Customer.Email,
                Name = token.Customer.Name,
                MobileNo = token.Customer.PhoneNumber,
                NoOfPerson = token.NoOfPersons,
                SectionId = token.SectionId
            };
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching token details", ex);
        }
    }

    public async Task<CustomerViewModel?> GetCustomerByEmail(string email)
    {
        try
        {
            Customer? customer = await _tableRepository.GetCustomerByEmail(email);
            if (customer == null) return null;

            var viewModel = new CustomerViewModel
            {
                Id = customer.Id,
                Email = customer.Email,
                Name = customer.Name,
                MobileNo = customer.PhoneNumber,
            };

            return viewModel;
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching token details", ex);
        }
    }

    public async Task<(bool IsSuccess, string Message)> AssignTablesAsync(AssignTableRequestViewModel model)
    {
        var selectedTables = await _tableRepository.GetTablesByIdsAsync(model.SelectedTables);
        int totalCapacity = selectedTables.Sum(t => t.Capacity);
        if (model.NumberOfPersons > totalCapacity)
        {
            return (false, $"Selected tables capacity is {totalCapacity}. Please select more tables.");

        }

        var customer = await _tableRepository.GetCustomerByEmail(model.Customer.Email);

        if (customer != null)
        {
            var existingCustomer = await _tableRepository.GetCustomerFromWaitingList(customer.Id);
            if (existingCustomer != null)
            {
                existingCustomer.IsAssign = true;
                existingCustomer.Updatedat = DateTime.Now;
            }


            var existingRunningOrder = await _tableRepository.GetRunningOrderByCustomerIdAsync(customer.Id);
            if (existingRunningOrder != null)
            {
                return (false, "This customer has a running order.");
            }

            if (customer.Name != model.Customer.Name || customer.PhoneNumber != model.Customer.MobileNo)
            {
                customer.Name = model.Customer.Name;
                customer.PhoneNumber = model.Customer.MobileNo;
                await _tableRepository.UpdateCustomerAsync(customer);
            }
        }
        else
        {
            customer = new Customer
            {
                Name = model.Customer.Name,
                Email = model.Customer.Email,
                PhoneNumber = model.Customer.MobileNo
            };
            await _tableRepository.CreateCustomerAsync(customer);
        }

        var order = new Order
        {
            Customerid = customer.Id,
            Status = "Pending",
            Createdat = DateTime.Now
        };

        await _tableRepository.CreateOrderAsync(order);

        var taxes = await _tableRepository.GetTaxesandfeesAsync();
        foreach (var tax in taxes)
        {
            var ordertax = new OrderTaxesMapping
            {
                OrderId = order.Id,
                TaxId = tax.Id,
                TaxName = tax.Name,
                TaxValue = tax.Value,
            };
            await _tableRepository.CreateOrderTaxAsync(ordertax);

        }


        int remainingPersons = model.NumberOfPersons;

        foreach (var table in selectedTables)
        {
            int personsForTable = Math.Min(remainingPersons, table.Capacity);

            await _tableRepository.CreateRangeAsync(new OrdersTableMapping
            {
                Orderid = order.Id,
                Tableid = table.Id,
                Name = table.Name,
                NoOfPerson = personsForTable
            });

            table.Status = "Assigned";
            await _tableRepository.UpdateTableStatusAsync(table);

            remainingPersons -= personsForTable;

            if (remainingPersons <= 0)
                break;

        }

        return (true, "Order created and tables assigned.");
    }

    public async Task<int?> GetOrderIdByTableIdAsync(int tableId)
    {
        return await _tableRepository.GetOrderIdByTableIdAsync(tableId);
    }



}



