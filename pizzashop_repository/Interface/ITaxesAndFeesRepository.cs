using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;

namespace pizzashop_repository.Interface;

public interface ITaxesAndFeesRepository
{
    Task<PagedResult<TaxsAndFeesViewModel>> GetTaxesAndFeesAsync(int pageNumber, int pageSize, string searchTerm = "");

     Task<bool> AddTaxAsync(Taxesandfee taxesandfee);

     Task<TaxsAndFeesViewModel> GetTaxByIdAsync(int id);

     Task<bool> SoftDeleteTaxAsync(int id);

     Task<bool> UpdateTaxesAsync(Taxesandfee taxesandfee);

     Task<Taxesandfee?> GetTaxByIdForEdit(int id);

     
}
