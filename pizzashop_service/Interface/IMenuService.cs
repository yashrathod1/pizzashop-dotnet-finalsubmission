using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;

namespace pizzashop_service.Interface;

public interface IMenuService
{
    Task<List<CategoryViewModel>> GetCategoriesAsync();

    Task<List<ItemViewModel>> GetItemsAsync();

    Task<Category> AddCategoryAsync(string name, string description, string Createdby);

    Task<CategoryViewModel?> GetCategoryByNameAsync(string name);

    Task<bool> UpdateCategoryAsync(Category category, string Updatedby);

    Task<CategoryViewModel?> GetCategoryByIdAsync(int id);

    Task<bool> SoftDeleteCategoryAsync(int id);

    // Task<List<ItemViewModel>> GetItemsByCategoryAsync(int categoryId);

    Task<bool> AddItemAsync(ItemViewModel model);

    Task<bool> UpdateItemAsync(ItemViewModel model, int id);

    Task<bool> SoftDeleteItemAsync(int id);

    void SoftDeleteItems(List<int> itemIds);

    ItemViewModel GetItemById(int id);

    Task<List<ModifierGroupViewModel>> GetModifierGroupAsync();

    Task<PagedResult<ModifierViewModel>> GetModifiersByModifierGroupAsync(int modifierGroupId, int pageNumber, int pageSize, string searchTerm = "");

    Task<List<ModifierViewModel>> GetModifiersAsync();

    Task<bool> AddModifierGroup(ModifierGroupViewModel model);

    Task<bool> SoftDeleteModifierGroupAsync(int id);

    Task<ModifierGroupViewModel> GetModifierGroupById(int id);

    Task<bool> UpdateModifierGroup(ModifierGroupViewModel model);

    Task<bool> AddModifierAsync(ModifierViewModel model);

    Task<ModifierViewModel> GetModifierById(int id);

    Task<bool> UpdateModifierAsync(ModifierViewModel model);

    Task<bool> SoftDeleteModifierAsync(int id);

    Task<bool> SoftDeleteModifierFromGroupAsync(int modifierId, int groupId);

    Task<bool> SoftDeleteModifiersAsync(List<int> modifierIds, int currentGroupId);

    Task<PagedResult<ItemViewModel>> GetItemsByCategoryAsync(int categoryId, int pageNumber, int pageSize, string searchTerm = "");
    Task<PagedResult<ModifierViewModel>> GetAllModifiersToAddModifierGroupAsync(int pageNumber, int pageSize, string searchTerm = "");

    Task<ModifierGroupViewModel?> GetModifiersByGroupIdAsync(int modifierGroupId);

    Task<ItemModifierGroupViewModel> GetModifierGroupByIdForItem(int groupId);
}
