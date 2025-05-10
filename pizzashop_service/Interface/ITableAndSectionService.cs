using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;

namespace pizzashop_service.Interface;

public interface ITableAndSectionService
{
    Task<List<SectionsViewModal>> GetSectionsAsync();

    Task<PagedResult<TableViewModel>> GetTableBySectionAsync(int SectionId, int pageNumber, int pageSize, string searchTerm = "");

    Task<Section> AddSectionAsync(string name, string description);

    Task<SectionsViewModal> GetSectionById(int id);
    Task<bool> UpdateSectionAsync(SectionsViewModal section);

    Task<SectionsViewModal?> GetSectionByNameAsync(string name);

    Task<bool> SoftDeleteSectionAsync(int id);

    Task<bool> AddTableAsync(TableViewModel model);

    Task<TableViewModel> GetTableById(int id);

    Task<TableViewModel?> GetTableByNameAsync(string name);

    Task<bool> UpdateTableAsync(TableViewModel model);

    Task<bool> SoftDeleteTableAsync(int id);

    void SoftDeleteTablesAsync(List<int> tableIds);
}
