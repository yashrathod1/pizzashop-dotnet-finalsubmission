using pizzashop_repository.Interface;
using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;
using pizzashop_service.Interface;

namespace pizzashop_service.Implementation;

public class WaitingListService : IWaitingListService
{
    private readonly IWaitingListRepository _waitingListRepository;

    public WaitingListService(IWaitingListRepository waitingListRepository)
    {
        _waitingListRepository = waitingListRepository;
    }

    public async Task<WaitingListViewModel> GetSectionAsync()
    {
        List<Section> section = await _waitingListRepository.GetSectionAsync();

        var WaitingList = await _waitingListRepository.GetAllWaitingList();

        WaitingListViewModel? viewModel = new WaitingListViewModel
        {
            Sections = section.Select(s => new WaitingListSectionViewModel
            {
                Id = s.Id,
                Name = s.Name,
                WaitingListCount = WaitingList.Count(w => w.SectionId == s.Id && w.IsAssign == false)
            }).ToList()
        };

        return viewModel;
    }

    public async Task<List<OrderAppSectionViewModel>> GetAllSectionsAsync()
    {
        var sections = await _waitingListRepository.GetSectionAsync();

        return sections.Select(s => new OrderAppSectionViewModel
        {
            Id = s.Id,
            Name = s.Name,
        }).ToList();
    }

    public async Task<WaitingListViewModel> GetWaitingListAsync(int? sectionId)
    {
        List<WaitingToken> waitingTokens = await _waitingListRepository.GetWaitingListAsync(sectionId);

        WaitingListViewModel? viewModel = new WaitingListViewModel
        {
            WaitingList = waitingTokens.Select(w => new WaitingListItemViewModel
            {
                Id = w.Id,
                CreatedAt = w.Createdat,
                WaitingTime = w.Createdat,
                Name = w.Customer.Name,
                Email = w.Customer.Email,
                NoOfPerson = w.NoOfPersons,
                PhoneNumber = w.Customer.PhoneNumber,
                CustomerId = w.CustomerId

            }).ToList(),

            SectionId = sectionId
        };

        return viewModel;
    }

    public async Task<bool> AddWaitingTokenInWaitingListAsync(WaitingTokenViewModel waitingTokenVm)
    {
        var existingCustomer = await _waitingListRepository.GetCustomerByEmail(waitingTokenVm.Email);

        Customer customer;

        if (existingCustomer == null)
        {
            customer = new Customer
            {
                Name = waitingTokenVm.Name,
                Email = waitingTokenVm.Email,
                PhoneNumber = waitingTokenVm.MobileNo
            };

            await _waitingListRepository.AddCustomer(customer);
        }
        else
        {
            customer = existingCustomer;

            var existingToken = await _waitingListRepository.GetWaitingTokenByCustomerId(customer.Id);
            if (existingToken != null)
            {
                return false;
            }
        }

        var waitingToken = new WaitingToken
        {
            CustomerId = customer.Id,
            NoOfPersons = waitingTokenVm.NoOfPerson,
            SectionId = waitingTokenVm.SectionId,
            Updatedat = DateTime.Now
        };

        var result = await _waitingListRepository.AddWaitingToken(waitingToken);

        return result;
    }

    public async Task<CustomerViewModel?> GetCustomerByEmail(string email)
    {
        try
        {
            Customer? customer = await _waitingListRepository.GetCustomerByEmail(email);
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

    public async Task<WaitingListItemViewModel?> GetTokenByIdAsync(int id)
    {
        var token = await _waitingListRepository.GetWaitingTokenById(id);

        if (token == null) return null;

        return new WaitingListItemViewModel
        {
            Id = token.Id,
            Name = token.Customer.Name,
            Email = token.Customer.Email,
            PhoneNumber = token.Customer.PhoneNumber,
            NoOfPerson = token.NoOfPersons,
            SectionId = token.SectionId

        };
    }

    public async Task<(bool success, string message)> EditWaitingTokenAsync(WaitingTokenViewModel model)
    {
        try
        {
            var waitingToken = await _waitingListRepository.GetWaitingTokenById(model.Id);

            if (waitingToken == null)
            {
                return (false, "Waiting token not found.");
            }

            if (waitingToken.Customer.Name == model.Name &&
                waitingToken.Customer.PhoneNumber == model.MobileNo &&
                waitingToken.SectionId == model.SectionId &&
                waitingToken.NoOfPersons == model.NoOfPerson &&
                waitingToken.Customer.Email == model.Email)
            {
                return (false, "No changes detected.");
            }

            waitingToken.Customer.Name = model.Name;
            waitingToken.Customer.PhoneNumber = model.MobileNo;
            waitingToken.SectionId = model.SectionId;
            waitingToken.NoOfPersons = model.NoOfPerson;
            waitingToken.Customer.Email = model.Email;

            await _waitingListRepository.UpdateAsync(waitingToken);

            return (true, "Waiting token updated successfully.");
        }
        catch (Exception ex)
        {
            throw new Exception("Error in updating waiting token", ex);
        }
    }

    public async Task<bool> SoftDeleteTokenAsync(int id)
    {
        try
        {
            var token = await _waitingListRepository.GetWaitingTokenById(id);
            if (token == null)
                return false;

            token.Isdeleted = true;

            await _waitingListRepository.UpdateAsync(token);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while soft deleting the token.", ex);
        }
    }

    public async Task<List<SectionsViewModal>> GetSectionsWithAvailableTablesAsync()
    {
        var sections = await _waitingListRepository.GetSectionsWithAvailableTablesAsync();
        return sections.Select(s => new SectionsViewModal
        {
            Id = s.Id,
            Name = s.Name
        }).ToList();
    }
    public async Task<List<TableViewModel>> GetAvailableTablesBySectionAsync(int sectionId)
    {
        var tables = await _waitingListRepository.GetAvailableTablesBySectionAsync(sectionId);
        return tables.Select(t => new TableViewModel
        {
            Id = t.Id,
            Name = t.Name
        }).ToList();
    }

    public async Task<AssignTableResultViewModel> AssignTablesToCustomerAsync(AssignTableInWaitingTokenViewModel model)
    {
        var selectedTables = await _waitingListRepository.GetTablesByIdsAsync(model.SelectedTables);
        int totalCapacity = selectedTables.Sum(t => t.Capacity);

        if (model.NumberOfPersons > totalCapacity)
        {
            return new AssignTableResultViewModel
            {
                IsSuccess = false,
                Message = $"Selected tables capacity is {totalCapacity}. Please select more tables."
            };
        }

        var customer = await _waitingListRepository.GetCustomerByIdAsync(model.CustomerId);

        var order = new Order
        {
            Customerid = customer.Id,
            Status = "Pending",
            Createdat = DateTime.Now
        };

        await _waitingListRepository.CreateOrderAsync(order);

        int remainingPersons = model.NumberOfPersons;

        foreach (var table in selectedTables)
        {
            int personsForTable = Math.Min(remainingPersons, table.Capacity);

            await _waitingListRepository.CreateRangeAsync(new OrdersTableMapping
            {
                Orderid = order.Id,
                Tableid = table.Id,
                Name = table.Name,
                NoOfPerson = personsForTable
            });

            table.Status = "Assigned";
            await _waitingListRepository.UpdateTableStatusAsync(table);

            remainingPersons -= personsForTable;
            if (remainingPersons <= 0) break;
        }

        var taxes = await _waitingListRepository.GetTaxesandfeesAsync();
        foreach (var tax in taxes)
        {
            var ordertax = new OrderTaxesMapping
            {
                OrderId = order.Id,
                TaxId = tax.Id,
                TaxName = tax.Name,
                TaxValue = tax.Value,
            };
            await _waitingListRepository.CreateOrderTaxAsync(ordertax);
        }

        var waitingCustomer = await _waitingListRepository.GetCustomerFromWaitingList(customer.Id);
        if (waitingCustomer != null)
        {
            waitingCustomer.IsAssign = true;
            await _waitingListRepository.UpdateWaitingCustomerAsync(waitingCustomer);
        }

        return new AssignTableResultViewModel
        {
            IsSuccess = true,
            Message = "Tables successfully assigned and order created.",
            OrderId = order.Id
        };
    }


}