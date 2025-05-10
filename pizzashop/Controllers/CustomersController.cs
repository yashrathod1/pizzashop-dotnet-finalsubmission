using Microsoft.AspNetCore.Mvc;
using pizzashop_repository.ViewModels;
using pizzashop_service.Interface;

namespace pizzashop.Controllers;

public class CustomersController : Controller
{
    private readonly ICustomersService _customersService;

    public CustomersController(ICustomersService customersService)
    {
        _customersService = customersService;
    }

    public IActionResult Index()
    {   
        ViewBag.ActiveNav = "Customers";
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers(int pageNumber = 1, int pageSize = 5, string sortBy = "Date", string sortOrder = "desc", string searchTerm = "", string dateRange = "All time", DateTime? customStartDate = null, DateTime? customEndDate = null)
    {
        PagedResult<CustomerTableViewModel>? customer = await _customersService.GetCustomersAsync(pageNumber, pageSize, sortBy, sortOrder, searchTerm, dateRange, customStartDate, customEndDate);
        return PartialView("_CustomerTablePartial", customer);
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomerHistory(int customerId)
    {

        CustomerHistoryViewModel? customerHistory = await _customersService.GetCustomerHistoryAsync(customerId);

        if (customerHistory == null)
        {
            return NotFound();
        }

        return Json(customerHistory);
    }

    [HttpGet]
    public async Task<IActionResult> ExportCustomer(string searchTerm = "", string dateRange = "All time", DateTime? customStartDate = null, DateTime? customEndDate = null)
    {
        byte[] fileContent = await _customersService.GenerateExcel(searchTerm, dateRange, customStartDate, customEndDate);

        if (fileContent == null || fileContent.Length == 0)
        {
            return Json("Failed to generate Excel file.");
        }
        string fileName = $"Customer_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(fileContent,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
    }
}
