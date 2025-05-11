using Microsoft.AspNetCore.Mvc;
using pizzashop_repository.ViewModels;
using pizzashop_service.Interface;

namespace pizzashop.Controllers;

public class WaitingListController : Controller
{
    private readonly IWaitingListService _waitingListService;

    public WaitingListController(IWaitingListService waitingListService)
    {
        _waitingListService = waitingListService;
    }

    public async Task<IActionResult> Index()
    {
        WaitingListViewModel? section = await _waitingListService.GetSectionAsync();
        ViewBag.ActiveNav = "WaitingList";
        return View(section);
    }

    [HttpGet]
    public async Task<IActionResult> GetSections()
    {
        List<OrderAppSectionViewModel>? sections = await _waitingListService.GetAllSectionsAsync();
        return Json(sections);
    }

    public async Task<IActionResult> GetWaitingListBySection(int? sectionId)
    {
        WaitingListViewModel? model = await _waitingListService.GetWaitingListAsync(sectionId);
        return PartialView("_WaitingListPartial", model.WaitingList);
    }

    [HttpPost]
    public async Task<IActionResult> CreateWaitingToken(WaitingTokenViewModel waitingTokenVm)
    {
        if (waitingTokenVm == null)
        {
            return Json("Invalid data.");
        }

        bool result = await _waitingListService.AddWaitingTokenInWaitingListAsync(waitingTokenVm);

        if (result)
        {
            return Json(new { success = true, message = "Waiting token added successfully!" });
        }
        else
        {
            return Json(new { success = false, message = "Waiting token already exits" });
        }

    }

    public async Task<IActionResult> GetCustomerByEmail(string email)
    {
        CustomerViewModel? customer = await _waitingListService.GetCustomerByEmail(email);
        return Json(customer);
    }

    [HttpGet]
    public async Task<IActionResult> GetTokenById(int id)
    {
        WaitingListItemViewModel? model = await _waitingListService.GetTokenByIdAsync(id);
        if (model != null)
        {
            return Json(model);
        }
        return Json(null);
    }


    [HttpPost]
    public async Task<IActionResult> EditWaitingToken(WaitingTokenViewModel model)
    {
        try
        {
            var result = await _waitingListService.EditWaitingTokenAsync(model);

            if (result.success)
            {
                return Json(new { success = true, message = result.message });
            }
            else
            {
                return Json(new { success = false, message = result.message });
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error in updating waiting token", ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> SoftDeleteToken(int id)
    {
        try
        {
            bool result = await _waitingListService.SoftDeleteTokenAsync(id);
            if (result)
            {
                return Json(new { success = true, message = "Token soft deleted successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Token not found." });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAvailableSectionsAsync()
    {
        List<SectionsViewModal>? sections = await _waitingListService.GetSectionsWithAvailableTablesAsync();
        return Json(sections);
    }
    [HttpGet]
    public async Task<IActionResult> GetAvailableTablesAsync(int sectionId)
    {
        List<TableViewModel>? tables = await _waitingListService.GetAvailableTablesBySectionAsync(sectionId);
        return Json(tables);
    }

    [HttpPost]
    public async Task<IActionResult> AssignTables([FromBody] AssignTableInWaitingTokenViewModel model)
    {
        AssignTableResultViewModel? result = await _waitingListService.AssignTablesToCustomerAsync(model);

        return Json(new
        {
            success = result.IsSuccess,
            message = result.Message,
            orderId = result.OrderId
        });
    }



}