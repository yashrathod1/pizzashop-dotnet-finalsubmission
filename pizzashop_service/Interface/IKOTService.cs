using pizzashop_repository.ViewModels;

namespace pizzashop_service.Interface;

public interface IKOTService
{
    Task<KOTViewModel> GetCategoryAsync();

    Task<KOTViewModel> GetKOTDataAsync(int? categoryId, string status);

    Task<KOTOrderCardViewModel?> GetOrderCardByIdAsync(int orderId, string status);

    Task UpdatePreparedQuantitiesAsync(UpdatePreparedItemsViewModel model);
}
