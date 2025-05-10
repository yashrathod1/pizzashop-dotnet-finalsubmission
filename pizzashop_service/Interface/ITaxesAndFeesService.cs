using pizzashop_repository.ViewModels;

namespace pizzashop_service.Interface;

public interface ITaxesAndFeesService
{
    Task<PagedResult<TaxsAndFeesViewModel>> GetTaxesAndFeesAsync(int pageNumber, int pageSize, string searchTerm = "");

    Task<bool> AddTaxAsync(TaxsAndFeesViewModel model);

    Task<TaxsAndFeesViewModel> GetTaxByIdAsync(int id);

    Task<bool> SoftDeleteTaxAsync(int id);

    Task<bool> UpdateTaxAsync(TaxsAndFeesViewModel model);
}
