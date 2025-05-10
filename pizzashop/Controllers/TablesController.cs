using Microsoft.AspNetCore.Mvc;
using pizzashop_repository.ViewModels;
using pizzashop_service.Interface;

namespace pizzashop.Controllers;

public class TablesController : Controller
{
    private readonly ITableService _tableService;

    public TablesController(ITableService tableService)
    {
        _tableService = tableService;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.ActiveNav = "Tables";
        var sectionandtable = await _tableService.GetSectionsWithTablesAsync();
        return View(sectionandtable);
    }

    [HttpGet]
    public async Task<IActionResult> GetSections()
    {
        var sections = await _tableService.GetAllSectionsAsync();
        return Json(sections);
    }

    [HttpPost]
    public async Task<IActionResult> AddWaitingToken(WaitingTokenViewModel waitingTokenVm)
    {
        if (waitingTokenVm == null)
        {
            return Json("Invalid data.");
        }

        var result = await _tableService.AddWaitingTokenAsync(waitingTokenVm);

        if (result)
        {
            return Json(new { message = "Waiting token added successfully!" });
        }
        else
        {
            return Json(new { message = "Failed to add waiting token." });
        }

    }

    [HttpGet]
    public async Task<IActionResult> GetWaitingTokenList(int sectionId)
    {
        var waitingTokens = await _tableService.GetWaitingTokens(sectionId);
        return PartialView("_TableWaitingListPartial", waitingTokens);
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomerDetailsByToken(int tokenId)
    {
        var customerDetails = await _tableService.GetCustomerDetailsByToken(tokenId);
        return Json(customerDetails);
    }

    public async Task<IActionResult> GetCustomerByEmail(string email)
    {
        var customer = await _tableService.GetCustomerByEmail(email);
        return Json(customer);
    }

    [HttpPost]
    public async Task<IActionResult> AssignTables([FromBody] AssignTableRequestViewModel model)
    {
        try
        {
            var result = await _tableService.AssignTablesAsync(model);
            return Json(new { success = result.IsSuccess, message = result.Message });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "An unexpected error occurred." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetOrderIdByTable(int tableId)
    {
        var orderId = await _tableService.GetOrderIdByTableIdAsync(tableId);
       return Json(orderId);
    }


}