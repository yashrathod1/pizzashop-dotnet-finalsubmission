using Microsoft.EntityFrameworkCore;
using pizzashop_repository.Interface;
using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;
using pizzashop_service.Interface;

namespace pizzashop_service.Implementation;

public class MenuService : IMenuService
{
    private readonly IMenuRepository _menuRepository;

    public MenuService(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<List<CategoryViewModel>> GetCategoriesAsync()
    {
        try
        {
            var categories = await _menuRepository.GetCategoriesAsync();

            var viewModels = categories.Select(x => new CategoryViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToList();

            return viewModels;
        }
        catch (Exception ex)
        {
            throw new Exception("Error mapping categories in service layer.", ex);
        }
    }

    public async Task<List<ItemViewModel>> GetItemsAsync()
    {
        try
        {
            var items = await _menuRepository.GetItemsAsync();

            var itemViewModels = items.Select(i => new ItemViewModel
            {
                Id = i.Id,
                Name = i.Name,
                Rate = i.Rate,
                Quantity = i.Quantity,
                IsAvailable = i.IsAvailable,
                ItemType = i.Type,
                ItemImagePath = i.ItemImage,
                Description = i.Description,
                ShortCode = i.ShortCode,
                IsDefaultTax = i.IsdefaultTax,
                Categoryid = i.Categoryid,
                Unit = i.UnitType,
                TaxPercentage = i.TaxPercentage
            }).ToList();

            return itemViewModels;
        }
        catch (Exception ex)
        {
            throw new Exception("Error mapping items in service layer.", ex);
        }
    }

    public async Task<Category> AddCategoryAsync(string name, string description, string createdBy)
    {
        try
        {
            Category category = new Category
            {
                Name = name,
                Createdby = createdBy,
                Description = description
            };

            return await _menuRepository.AddCategoryAsync(category);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while adding the category.", ex);
        }
    }


    public async Task<CategoryViewModel?> GetCategoryByNameAsync(string name)
    {
        try
        {
            Category? category = await _menuRepository.GetCategoryByNameAsync(name);
            return category != null
                ? new CategoryViewModel { Name = category.Name, Id = category.Id }
                : null;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving the category by name.", ex);
        }
    }

    public async Task<bool> UpdateCategoryAsync(Category category, string updatedBy)
    {
        try
        {
            Category? existingCategory = await _menuRepository.GetCategoryByIdAsync(category.Id);
            if (existingCategory == null)
                return false;

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            existingCategory.Updatedby = updatedBy;

            return await _menuRepository.UpdateCategoryAsync(existingCategory);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred in the service layer while updating the category.", ex);
        }
    }

    public async Task<CategoryViewModel?> GetCategoryByIdAsync(int id)
    {
        try
        {
            Category? category = await _menuRepository.GetCategoryByIdAsync(id);
            if (category == null) return null;

            return new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving the category by ID.", ex);
        }
    }

    public async Task<bool> SoftDeleteCategoryAsync(int id)
    {
        try
        {
            Category? category = await _menuRepository.GetCategoryByIdAsync(id);
            if (category == null) return false;

            category.Isdeleted = true;
            return await _menuRepository.SoftDeleteCategoryAsync(category);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred in the service layer while soft deleting the category.", ex);
        }
    }


    // public async Task<List<ItemViewModel>> GetItemsByCategoryAsync(int categoryId)
    // {
    //     try
    //     {
    //         var menuItems = await _menuRepository.GetItemsByCategoryAsync(categoryId);

    //         var itemViewModels = menuItems.Select(i => new ItemViewModel
    //         {
    //             Id = i.Id,
    //             Name = i.Name,
    //             Rate = i.Rate,
    //             Quantity = i.Quantity,
    //             IsAvailable = i.IsAvailable,
    //             ItemType = i.Type,
    //             ItemImagePath = i.ItemImage,
    //             Description = i.Description,
    //             ShortCode = i.ShortCode,
    //             IsDefaultTax = i.IsdefaultTax,
    //             Categoryid = i.Categoryid,
    //             Unit = i.UnitType,
    //             TaxPercentage = i.TaxPercentage,
    //         }).ToList();

    //         return itemViewModels;
    //     }
    //     catch (Exception ex)
    //     {
    //         throw new Exception("An error occurred in the service layer while fetching items by category.", ex);
    //     }
    // }


    public async Task<PagedResult<ItemViewModel>> GetItemsByCategoryAsync(int categoryId, int pageNumber, int pageSize, string searchTerm = "")
    {
        try
        {
            var query = await _menuRepository.GetItemsByCategoryQueryAsync(categoryId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(i => i.Name.ToLower().Contains(searchTerm.ToLower()));
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(i => i.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new ItemViewModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    Rate = i.Rate,
                    Quantity = i.Quantity,
                    IsAvailable = i.IsAvailable,
                    ItemType = i.Type,
                    ItemImagePath = i.ItemImage,
                    Description = i.Description,
                    ShortCode = i.ShortCode,
                    IsDefaultTax = i.IsdefaultTax,
                    Categoryid = i.Categoryid,
                    Unit = i.UnitType,
                    TaxPercentage = i.TaxPercentage
                })
                .ToListAsync();

            return new PagedResult<ItemViewModel>(items, pageNumber, pageSize, totalCount);
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred while fetching or processing items", ex);
        }
    }

    public async Task<bool> AddItemAsync(ItemViewModel model)
    {
        try
        {
            MenuItem? newItem = new MenuItem
            {
                Name = model.Name,
                Categoryid = model.Categoryid,
                Type = model.ItemType,
                Rate = model.Rate,
                Quantity = model.Quantity,
                UnitType = model.Unit,
                IsAvailable = model.IsAvailable,
                IsdefaultTax = model.IsDefaultTax,
                TaxPercentage = model.TaxPercentage,
                ShortCode = model.ShortCode,
                Description = model.Description,
                ItemImage = model.ItemImagePath
            };

            bool itemAdded = await _menuRepository.AddItemAsync(newItem);

            if (itemAdded && model.ModifierGroups != null && model.ModifierGroups.Any())
            {
                List<MappingMenuItemWithModifier>? modifierMappings = model.ModifierGroups.Select(mg => new MappingMenuItemWithModifier
                {
                    MenuItemId = newItem.Id,
                    ModifierGroupId = mg.GroupId,
                    MinModifierCount = mg.MinQuantity,
                    MaxModifierCount = mg.MaxQuantity
                }).ToList();

                return await _menuRepository.AddItemModifiersAsync(modifierMappings);
            }

            return itemAdded;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while adding the item.", ex);
        }
    }

    public async Task<bool> UpdateItemAsync(ItemViewModel model, int id)
    {
        try
        {
            MenuItem? item = _menuRepository.GetItemsById(id);
            if (item == null) return false;

            List<MappingMenuItemWithModifier>? existingModifiers = await _menuRepository.GetItemWithModifiersByItemIdAsync(id);

            List<MappingMenuItemWithModifier>? newModifiers = model.ModifierGroups?.Select(mg => new MappingMenuItemWithModifier
            {
                MenuItemId = item.Id,
                ModifierGroupId = mg.GroupId,
                MinModifierCount = mg.MinQuantity,
                MaxModifierCount = mg.MaxQuantity
            }).ToList() ?? new List<MappingMenuItemWithModifier>();

            bool itemChanged =
                item.Name != model.Name ||
                item.Categoryid != model.Categoryid ||
                item.Type != model.ItemType ||
                item.Rate != model.Rate ||
                item.Quantity != model.Quantity ||
                item.UnitType != model.Unit ||
                item.IsAvailable != model.IsAvailable ||
                item.IsdefaultTax != model.IsDefaultTax ||
                item.TaxPercentage != model.TaxPercentage ||
                item.ShortCode != model.ShortCode ||
                item.Description != model.Description ||
                item.ItemImage != model.ItemImagePath;

            bool modifiersChanged =
                existingModifiers.Count != newModifiers.Count ||
                existingModifiers.Any(em =>
                    !newModifiers.Any(nm =>
                        nm.ModifierGroupId == em.ModifierGroupId &&
                        nm.MinModifierCount == em.MinModifierCount &&
                        nm.MaxModifierCount == em.MaxModifierCount
                    )
                ) ||
                newModifiers.Any(nm =>
                    !existingModifiers.Any(em =>
                        em.ModifierGroupId == nm.ModifierGroupId &&
                        em.MinModifierCount == nm.MinModifierCount &&
                        em.MaxModifierCount == nm.MaxModifierCount
                    )
                );

            if (!itemChanged && !modifiersChanged)
            {
                return false;
            }

            if (itemChanged)
            {
                item.Name = model.Name;
                item.Categoryid = model.Categoryid;
                item.Type = model.ItemType;
                item.Rate = model.Rate;
                item.Quantity = model.Quantity;
                item.UnitType = model.Unit;
                item.IsAvailable = model.IsAvailable;
                item.IsdefaultTax = model.IsDefaultTax;
                item.TaxPercentage = model.TaxPercentage;
                item.ShortCode = model.ShortCode;
                item.Description = model.Description;
                item.ItemImage = model.ItemImagePath;

                bool itemUpdated = await _menuRepository.UpdateItemAsync(item);
                if (!itemUpdated) return false;
            }

            if (modifiersChanged)
            {
                List<MappingMenuItemWithModifier>? modifiersToAdd = newModifiers
                    .Where(nm => !existingModifiers.Any(em => em.ModifierGroupId == nm.ModifierGroupId))
                    .ToList();

                List<MappingMenuItemWithModifier>? modifiersToRemove = existingModifiers
                    .Where(em => !newModifiers.Any(nm => nm.ModifierGroupId == em.ModifierGroupId))
                    .ToList();

                List<MappingMenuItemWithModifier>? modifiersToUpdate = existingModifiers
                    .Where(em => newModifiers.Any(nm =>
                        nm.ModifierGroupId == em.ModifierGroupId &&
                        (nm.MinModifierCount != em.MinModifierCount || nm.MaxModifierCount != em.MaxModifierCount)
                    ))
                    .Select(em => newModifiers.First(nm => nm.ModifierGroupId == em.ModifierGroupId))
                    .ToList();

                if (modifiersToAdd.Any())
                {
                    await _menuRepository.AddItemModifiersAsync(modifiersToAdd);
                }

                if (modifiersToRemove.Any())
                {
                    await _menuRepository.DeleteItemModifiersAsync(modifiersToRemove);
                }

                if (modifiersToUpdate.Any())
                {
                    await _menuRepository.UpdateItemModifiersAsync(modifiersToUpdate);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating the item.", ex);
        }
    }

    public async Task<bool> SoftDeleteItemAsync(int id)
    {
        try
        {
            return await _menuRepository.SoftDeleteItemAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while trying to soft delete the item.", ex);
        }
    }


    public void SoftDeleteItems(List<int> itemIds)
    {
        try
        {
            _menuRepository.SoftDeleteItemsAsync(itemIds);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while trying to soft delete the items.", ex);
        }
    }


    public ItemViewModel GetItemById(int id)
    {
        try
        {
            MenuItem? menuItem = _menuRepository.GetItemsById(id);
            if (menuItem == null)
                return null;

            List<MappingMenuItemWithModifier> modifierMappings = _menuRepository.GetItemModifiersByItemId(id);

            List<int> assignedModifierGroups = modifierMappings
                .Select(m => m.ModifierGroupId)
                .ToList();

            List<ItemModifierGroupViewModel> modifierGroups = modifierMappings
                .Select(m => new ItemModifierGroupViewModel
                {
                    Id = m.ModifierGroup.Id,
                    Name = m.ModifierGroup.Name,
                    MinQuantity = m.MinModifierCount,
                    MaxQuantity = m.MaxModifierCount,
                    AvailableModifiersForItem = m.ModifierGroup.Modifiergroupmodifiers
                        .Where(mgm => !mgm.Isdeleted && !mgm.Modifier.Isdeleted)
                        .Select(mgm => new ModifierViewModel
                        {
                            Id = mgm.Modifier.Id,
                            Name = mgm.Modifier.Name,
                            Price = mgm.Modifier.Price
                        }).ToList(),
                     ModifierCount = m.ModifierGroup.Modifiergroupmodifiers.Count(mgm => !mgm.Isdeleted && !mgm.Modifier.Isdeleted)
                }).ToList();

            ItemViewModel itemViewModel = new ItemViewModel
            {
                Id = menuItem.Id,
                Categoryid = menuItem.Categoryid,
                Name = menuItem.Name,
                Rate = menuItem.Rate,
                Unit = menuItem.UnitType,
                Quantity = menuItem.Quantity,
                ItemType = menuItem.Type,
                IsAvailable = menuItem.IsAvailable,
                ShortCode = menuItem.ShortCode,
                Description = menuItem.Description,
                TaxPercentage = menuItem.TaxPercentage,
                IsDefaultTax = menuItem.IsdefaultTax == true,
                ItemImagePath = menuItem.ItemImage,
                AssignedModifierGroups = assignedModifierGroups,
                ModifierGroups = modifierGroups
            };

            return itemViewModel;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while building ItemViewModel", ex);
        }
    }

    public async Task<List<ModifierGroupViewModel>> GetModifierGroupAsync()
    {
        try
        {
            List<Modifiergroup> modifierGroups = await _menuRepository.GetModifierGroupsAsync();

            List<ModifierGroupViewModel> modifierGroupViewModels = modifierGroups
                .Select(x => new ModifierGroupViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                }).ToList();

            return modifierGroupViewModels;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to map Modifier Groups to ViewModel", ex);
        }
    }


    public async Task<PagedResult<ModifierViewModel>> GetModifiersByModifierGroupAsync(
    int modifierGroupId, int pageNumber, int pageSize, string searchTerm = "")
    {
        try
        {
            IQueryable<Modifier> query = await _menuRepository.GetModifiersByModifierGroupQueryAsync(modifierGroupId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(m => m.Name.ToLower().Contains(searchTerm.ToLower()));
            }

            int totalCount = await query.CountAsync();

            List<ModifierViewModel> modifiers = await query
                .OrderBy(m => m.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new ModifierViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    Price = m.Price,
                    Quantity = m.Quantity,
                    Unittype = m.Unittype,
                    Description = m.Description,
                    Isdeleted = m.Isdeleted
                }).ToListAsync();

            return new PagedResult<ModifierViewModel>(modifiers, pageNumber, pageSize, totalCount);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to fetch modifiers", ex);
        }
    }



    public async Task<List<ModifierViewModel>> GetModifiersAsync()
    {
        try
        {
            var modifiers = await _menuRepository.GetModifiersAsync();
            var modifierViewModels = new List<ModifierViewModel>();

            foreach (var modifier in modifiers)
            {
                var modifierGroupIds = await _menuRepository.GetModifierGroupIdsForModifierAsync(modifier.Id);

                var modifierViewModel = new ModifierViewModel
                {
                    Id = modifier.Id,
                    Name = modifier.Name,
                    Price = modifier.Price,
                    Quantity = modifier.Quantity,
                    Unittype = modifier.Unittype,
                    Description = modifier.Description,
                    ModifierGroupIds = modifierGroupIds,
                    Isdeleted = modifier.Isdeleted
                };

                modifierViewModels.Add(modifierViewModel);
            }

            return modifierViewModels;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to retrieve modifiers", ex);
        }
    }

    public async Task<bool> AddModifierGroup(ModifierGroupViewModel model)
    {
        try
        {
            if (await _menuRepository.ExistsModifierGroupByNameAsync(model.Name))
            {
                return false;
            }

            Modifiergroup? modifierGroup = new Modifiergroup
            {
                Name = model.Name,
                Description = model.Description
            };

            return await _menuRepository.AddModifierGroup(modifierGroup, model.ModifierIds);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while trying to add the modifier group.", ex);
        }
    }


    public async Task<bool> SoftDeleteModifierGroupAsync(int id)
    {
        try
        {
            return await _menuRepository.SoftDeleteModifierGroupAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while trying to soft delete the modifier group.", ex);
        }
    }


    public async Task<ModifierGroupViewModel> GetModifierGroupById(int id)
    {
        try
        {
            Modifiergroup? modifierGroup = await _menuRepository.GetModifierGroupByIdAsync(id);

            if (modifierGroup == null)
                return null;

            var availableModifiers = modifierGroup.Modifiergroupmodifiers
                .Where(mgm => !mgm.Modifier.Isdeleted && !mgm.Isdeleted)
                .Select(mgm => new ModifierViewModel
                {
                    Id = mgm.Modifier.Id,
                    Name = mgm.Modifier.Name
                }).ToList();

            var modifierIds = modifierGroup.Modifiergroupmodifiers
                .Where(mgm => !mgm.Modifier.Isdeleted && !mgm.Isdeleted)
                .Select(mgm => mgm.Modifierid)
                .ToList();

            return new ModifierGroupViewModel
            {
                Id = modifierGroup.Id,
                Name = modifierGroup.Name,
                Description = modifierGroup.Description,
                AvailableModifiers = availableModifiers,
                ModifierIds = modifierIds
            };
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to retrieve modifier group by ID", ex);
        }
    }

    public async Task<bool> UpdateModifierGroup(ModifierGroupViewModel model)
    {
        try
        {
            Modifiergroup? existingModifierGroup = await _menuRepository.GetModifierGroupByIdAsync(model.Id);
            if (existingModifierGroup == null)
            {
                return false;
            }

            if (existingModifierGroup.Name == model.Name
                && existingModifierGroup.Description == model.Description
                && existingModifierGroup.Modifiergroupmodifiers.Select(mgm => mgm.Modifierid).OrderBy(id => id)
                    .SequenceEqual(model.ModifierIds.OrderBy(id => id)))
            {
                return false;
            }

            return await _menuRepository.UpdateModifierGroup(model);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating the modifier group", ex);
        }
    }

    public async Task<bool> AddModifierAsync(ModifierViewModel model)
    {
        try
        {
            var modifier = new Modifier
            {
                Name = model.Name,
                Price = model.Price,
                Unittype = model.Unittype,
                Quantity = model.Quantity,
                Description = model.Description,
                Isdeleted = false
            };

            return await _menuRepository.AddModifierAsync(modifier, model.ModifierGroupIds);
        }
        catch (Exception ex)
        {
            throw new Exception("Error while adding the modifier", ex);
        }
    }

    // Service Layer
    public async Task<ModifierViewModel> GetModifierById(int id)
    {
        try
        {
            Modifier? modifier = await _menuRepository.GetModifierById(id);
            if (modifier == null)
            {
                return null;
            }

            List<int> modifierGroupIds = await _menuRepository.GetModifierGroupIdsByModifierId(modifier.Id);

            ModifierViewModel modifierViewModel = new ModifierViewModel
            {
                Id = modifier.Id,
                Name = modifier.Name,
                Price = modifier.Price,
                Unittype = modifier.Unittype,
                Quantity = modifier.Quantity,
                Description = modifier.Description,
                Isdeleted = modifier.Isdeleted,
                ModifierGroupIds = modifierGroupIds
            };

            return modifierViewModel;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while fetching the modifier details.", ex);
        }
    }

    public async Task<bool> UpdateModifierAsync(ModifierViewModel model)
    {
        try
        {
            Modifier? modifier = await _menuRepository.GetModifierByIdAsync(model.Id);
            if (modifier == null) return false;

            bool isModified = modifier.Isdeleted;
            if (modifier.Isdeleted)
            {
                modifier.Isdeleted = false;
                isModified = true;
            }

            if (modifier.Name != model.Name ||
                modifier.Price != model.Price ||
                modifier.Quantity != model.Quantity ||
                modifier.Unittype != model.Unittype ||
                modifier.Description != model.Description)
            {
                modifier.Name = model.Name;
                modifier.Price = model.Price;
                modifier.Quantity = model.Quantity;
                modifier.Unittype = model.Unittype;
                modifier.Description = model.Description;
                isModified = true;
            }

            List<int>? existingGroupIds = await _menuRepository.GetModifierGroupIdsByModifierId(model.Id);
            if (!existingGroupIds.OrderBy(x => x).SequenceEqual(model.ModifierGroupIds.OrderBy(x => x)))
            {
                await _menuRepository.UpdateModifierGroupsAsync(model.Id, model.ModifierGroupIds);
                isModified = true;
            }

            if (!isModified)
            {
                return false;
            }

            return await _menuRepository.UpdateModifierAsync(modifier);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating the modifier.", ex);
        }
    }



    public async Task<bool> SoftDeleteModifierAsync(int id)
    {
        try
        {
            return await _menuRepository.SoftDeleteModifierAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while soft deleting the modifier.", ex);
        }
    }


    public async Task<bool> SoftDeleteModifierFromGroupAsync(int modifierId, int groupId)
    {
        return await _menuRepository.SoftDeleteModifierFromGroupAsync(modifierId, groupId);
    }

    public async Task<bool> SoftDeleteModifiersAsync(List<int> modifierIds, int currentGroupId)
    {
        return await _menuRepository.SoftDeleteModifiersAsync(modifierIds, currentGroupId);
    }
    public async Task<PagedResult<ModifierViewModel>> GetAllModifiersToAddModifierGroupAsync(int pageNumber, int pageSize, string searchTerm = "")
    {
        return await _menuRepository.GetAllModifiersToAddModifierGroupAsync(pageNumber, pageSize, searchTerm);
    }

    public async Task<ModifierGroupViewModel?> GetModifiersByGroupIdAsync(int modifierGroupId)
    {
        return await _menuRepository.GetModifiersByGroupIdAsync(modifierGroupId);
    }

    public async Task<ItemModifierGroupViewModel> GetModifierGroupByIdForItem(int groupId)
    {
        return await _menuRepository.GetModifierGroupByIdForItem(groupId);
    }
}
