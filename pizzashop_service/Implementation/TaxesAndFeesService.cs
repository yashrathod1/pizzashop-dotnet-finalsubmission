using pizzashop_repository.Interface;
using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;
using pizzashop_service.Interface;

namespace pizzashop_service.Implementation;

public class TaxesAndFeesService : ITaxesAndFeesService
{

    private readonly ITaxesAndFeesRepository _taxesAndFeesRepository;

    public TaxesAndFeesService(ITaxesAndFeesRepository taxesAndFeesService)
    {
        _taxesAndFeesRepository = taxesAndFeesService;
    }
    public async Task<PagedResult<TaxsAndFeesViewModel>> GetTaxesAndFeesAsync(int pageNumber, int pageSize, string searchTerm = "")
    {
        return await _taxesAndFeesRepository.GetTaxesAndFeesAsync(pageNumber, pageSize, searchTerm);
    }

    public async Task<bool> AddTaxAsync(TaxsAndFeesViewModel model)
    {

        Taxesandfee? tax = new Taxesandfee
        {
            Name = model.Name,
            Type = model.Type,
            Value = model.Value,
            Isenabled = model.IsEnabled,
            Isdefault = model.IsDefault
        };

        return await _taxesAndFeesRepository.AddTaxAsync(tax);
    }

    public async Task<TaxsAndFeesViewModel> GetTaxByIdAsync(int id)
    {
        return await _taxesAndFeesRepository.GetTaxByIdAsync(id);
    }

    public async Task<bool> SoftDeleteTaxAsync(int id)
    {
        return await _taxesAndFeesRepository.SoftDeleteTaxAsync(id);
    }

      public async Task<bool> UpdateTaxAsync(TaxsAndFeesViewModel model)
    {
        Taxesandfee? tax = await _taxesAndFeesRepository.GetTaxByIdForEdit(model.Id);
        if (tax == null) return false;

        tax.Name = model.Name;
        tax.Type = model.Type;
        tax.Value = model.Value;
        tax.Isenabled = model.IsEnabled;
        tax.Isdefault = model.IsDefault;

        return await _taxesAndFeesRepository.UpdateTaxesAsync(tax);
    }
}
