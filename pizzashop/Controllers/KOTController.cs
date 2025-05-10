using Microsoft.AspNetCore.Mvc;
using pizzashop_repository.ViewModels;
using pizzashop_service.Interface;

namespace pizzashop.Controllers;

public class KOTController : Controller
{
    private readonly IKOTService _kOTService;

    public KOTController(IKOTService kOTService)
    {
        _kOTService = kOTService;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.ActiveNav = "KOT";
        KOTViewModel? kotData = await _kOTService.GetCategoryAsync();
        return View(kotData);
    }

    public async Task<IActionResult> GetOrderCardByCategory(int? categoryId, string status)
    {
        KOTViewModel? kotData = await _kOTService.GetKOTDataAsync(categoryId,status);
        List<KOTOrderCardViewModel>? filteredOrders = kotData.OrderCard;
        string noOrderMessage = status == "Ready" ? "No orders available in Ready" : "No orders available in In Progress";
        ViewBag.noOrderMessage = filteredOrders == null || !filteredOrders.Any() ? noOrderMessage : string.Empty;

        return PartialView("_OrderCardsSliderPartial", filteredOrders);
    }


    public async Task<IActionResult> GetOrderCardInModal(int orderId, string status)
    {
        KOTOrderCardViewModel? model = await _kOTService.GetOrderCardByIdAsync(orderId, status);
      
        return PartialView("_KOTOrderItemModalPartial", model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePreparedQuantities([FromBody] UpdatePreparedItemsViewModel model)
    {
        await _kOTService.UpdatePreparedQuantitiesAsync(model);
        return Ok();
    }

}
