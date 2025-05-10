using Microsoft.AspNetCore.Mvc;
using pizzashop_repository.ViewModels;
using pizzashop_service.Interface;

namespace pizzashop.Controllers;

public class OrdersController : Controller
{
    private readonly IOredersService _oredersService;

    public OrdersController(IOredersService oredersService)
    {
        _oredersService = oredersService;
    }

    public IActionResult Index()
    {   
        ViewBag.ActiveNav = "Orders";
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders(int pageNumber = 1, int pageSize = 5, string sortBy = "Id", string sortOrder = "asc", string searchTerm = "", string status = "All", string dateRange = "All time", string fromDate = "", string toDate = "")
    {
        PagedResult<OrdersTableViewModel>? pagedItems = await _oredersService.GetOrdersAsync(pageNumber, pageSize, sortBy, sortOrder, searchTerm, status, dateRange, fromDate, toDate);
        return PartialView("_OrderTablePartial", pagedItems);
    }

    [HttpGet]
    public IActionResult ExportOrders(string status, string date, string searchText)
    {
        status ??= "";
        date ??= "";
        searchText ??= "";

        byte[] fileContent = _oredersService.GenerateExcel(status, date, searchText);

        if (fileContent == null || fileContent.Length == 0)
        {
            return Json("Failed to generate Excel file.");
        }

        string fileName = $"Orders_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(fileContent,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
    }

    public async Task<IActionResult> OrderDetails(int id)
    {
        OrderDetailsViewModel? orderDetails = await _oredersService.GetOrderDetailsAsync(id);
        if (orderDetails == null)
        {
            return NotFound();
        }
        return View(orderDetails);
    }


    public async Task<IActionResult> GenerateInvoicePdf(int id)
    {
        try
        {
            byte[] pdfBytes = await _oredersService.GenerateInvoicePdfAsync(id);
            return File(pdfBytes, "application/pdf", $"Invoice_{id}.pdf");
        }
        catch (Exception ex)
        {
            return Json(ex.Message);
        }
    }

    public async Task<IActionResult> GenerateOrderDetailsPdf(int id)
    {
        try
        {
            byte[] pdfBytes = await _oredersService.GenerateOrderDetailsPdfAsync(id);
            return File(pdfBytes, "application/pdf", $"Order-details_{id}.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
