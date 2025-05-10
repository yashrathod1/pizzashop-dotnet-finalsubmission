using Microsoft.EntityFrameworkCore;
using pizzashop_repository.Database;
using pizzashop_repository.Interface;
using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;

namespace pizzashop_repository.Implementation;

public class MenuRepository : IMenuRepository
{
    private readonly PizzaShopDbContext _context;

    public MenuRepository(PizzaShopDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        try
        {
            return await _context.Categories
                .Where(c => !c.Isdeleted)
                .OrderBy(c => c.Id)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching categories from database.", ex);
        }
    }

    public async Task<List<MenuItem>> GetItemsAsync()
    {
        try
        {
            return await _context.MenuItems
                .Where(i => !i.IsDeleted)
                .OrderBy(i => i.Id)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching items from database.", ex);
        }
    }


    public async Task<Category> AddCategoryAsync(Category category)
    {
        try
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while adding the category.", ex);
        }
    }


    public async Task<Category?> GetCategoryByNameAsync(string name)
    {
        try
        {
            return await _context.Categories
                .Where(c => !c.Isdeleted)
                .FirstOrDefaultAsync(c => c.Name == name);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while fetching the category by name.", ex);
        }
    }



    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        try
        {
            return await _context.Categories.FindAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while fetching the category by ID.", ex);
        }
    }

    public async Task<bool> UpdateCategoryAsync(Category category)
    {
        try
        {
            _context.Categories.Update(category);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating the category.", ex);
        }
    }


    // public async Task<Category?> GetCategoryByIdAsync(int id)
    // {
    //     return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    // }

    public async Task<bool> SoftDeleteCategoryAsync(Category category)
    {
        try
        {
            _context.Categories.Update(category);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while soft deleting the category.", ex);
        }
    }

    // public async Task<List<MenuItem>> GetItemsByCategoryAsync(int categoryId)
    // {
    //     try
    //     {
    //         return await _context.MenuItems
    //             .Where(i => i.Categoryid == categoryId && !i.IsDeleted)
    //             .ToListAsync();
    //     }
    //     catch (Exception ex)
    //     {
    //         throw new Exception("An error occurred while fetching items by category.", ex);
    //     }
    // }


    public async Task<IQueryable<MenuItem>> GetItemsByCategoryQueryAsync(int categoryId)
    {
        try
        {
            var query = _context.MenuItems
                .Where(i => i.Categoryid == categoryId && !i.IsDeleted)
                .AsQueryable();

            return await Task.FromResult(query);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to retrieve items from database", ex);
        }
    }

    public async Task<bool> AddItemAsync(MenuItem item)
    {
        try
        {
            _context.MenuItems.Add(item);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while adding the item to the database", ex);
        }
    }


    public async Task<bool> AddItemModifiersAsync(List<MappingMenuItemWithModifier> mappings)
    {
        try
        {
            _context.MappingMenuItemWithModifiers.AddRange(mappings);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while adding item modifiers to the database", ex);
        }
    }


    public async Task<bool> UpdateItemAsync(MenuItem item)
    {
        try
        {
            _context.MenuItems.Update(item);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating the item in the database", ex);
        }
    }


    public async Task<bool> DeleteItemModifiersAsync(List<MappingMenuItemWithModifier> modifiersToRemove)
    {
        try
        {
            if (modifiersToRemove == null || !modifiersToRemove.Any())
                return false;

            _context.MappingMenuItemWithModifiers.RemoveRange(modifiersToRemove);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while deleting item modifiers from the database", ex);
        }
    }

    public MenuItem? GetItemsById(int id)
    {
        try
        {
            return _context.MenuItems.FirstOrDefault(x => x.Id == id);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving the item by ID from the database", ex);
        }
    }


    public async Task<bool> SoftDeleteItemAsync(int id)
    {
        try
        {
            MenuItem? item = await _context.MenuItems.FindAsync(id);
            if (item == null) return false;

            item.IsDeleted = true;
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while soft deleting the item", ex);
        }
    }

    public void SoftDeleteItemsAsync(List<int> itemIds)
    {
        try
        {
            List<MenuItem>? items = _context.MenuItems.Where(x => itemIds.Contains(x.Id)).ToList();
            foreach (MenuItem? item in items)
            {
                item.IsDeleted = true;
            }
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while soft deleting multiple items", ex);
        }
    }

    public List<MappingMenuItemWithModifier> GetItemModifiersByItemId(int itemId)
    {
        try
        {
            return _context.MappingMenuItemWithModifiers
                .Where(m => m.MenuItemId == itemId)
                .Include(m => m.ModifierGroup)
                    .ThenInclude(g => g.Modifiergroupmodifiers)
                        .ThenInclude(mgm => mgm.Modifier)
                .ToList();
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to fetch item modifiers", ex);
        }
    }

    public async Task<List<Modifiergroup>> GetModifierGroupsAsync()
    {
        try
        {
            return await _context.Modifiergroups
                .Where(c => !c.Isdeleted)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to fetch Modifier Groups", ex);
        }
    }


    public async Task<IQueryable<Modifier>> GetModifiersByModifierGroupQueryAsync(int modifierGroupId)
    {
        try
        {
            IQueryable<Modifier> query = _context.Modifiergroupmodifiers
                .Where(mgm => mgm.Modifiergroupid == modifierGroupId && !mgm.Modifier.Isdeleted && !mgm.Isdeleted)
                .Select(mgm => mgm.Modifier);

            return await Task.FromResult(query);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to retrieve modifiers from database", ex);
        }
    }

    public async Task<List<Modifier>> GetModifiersAsync()
    {
        try
        {
            return await _context.Modifiers
                .Where(m => !m.Isdeleted)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to retrieve modifiers from database", ex);
        }
    }

    public async Task<List<int>> GetModifierGroupIdsForModifierAsync(int modifierId)
    {
        try
        {
            return await _context.Modifiergroupmodifiers
                .Where(mgm => mgm.Modifierid == modifierId)
                .Select(mgm => mgm.Modifiergroupid)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to retrieve modifier group IDs for modifier {modifierId}", ex);
        }
    }


    public async Task<bool> AddModifierGroup(Modifiergroup modifierGroup, List<int> modifierIds)
    {
        try
        {
            await _context.Modifiergroups.AddAsync(modifierGroup);
            await _context.SaveChangesAsync();

            if (modifierIds != null && modifierIds.Count > 0)
            {
                foreach (int modifierId in modifierIds)
                {
                    Modifiergroupmodifier? modifierGroupModifier = new Modifiergroupmodifier
                    {
                        Modifiergroupid = modifierGroup.Id,
                        Modifierid = modifierId
                    };
                    await _context.Modifiergroupmodifiers.AddAsync(modifierGroupModifier);
                }
                await _context.SaveChangesAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to add modifier group", ex);
        }
    }


    public async Task<bool> ExistsModifierGroupByNameAsync(string name)
    {
        try
        {
            return await _context.Modifiergroups.AnyAsync(mg => mg.Name.ToLower() == name.ToLower());
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to check if modifier group exists by name", ex);
        }
    }

    public async Task<bool> SoftDeleteModifierGroupAsync(int id)
    {
        try
        {
            Modifiergroup? modifierGroup = await _context.Modifiergroups.FindAsync(id);
            List<Modifiergroupmodifier>? modifierGroupModifier = await _context.Modifiergroupmodifiers
                .Where(mgm => mgm.Modifiergroupid == id)
                .ToListAsync();

            if (modifierGroup == null) return false;

            foreach (Modifiergroupmodifier? modifiergroup in modifierGroupModifier)
            {
                modifiergroup.Isdeleted = true;
            }

            modifierGroup.Isdeleted = true;
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to soft delete the modifier group", ex);
        }
    }


    public async Task<Modifiergroup?> GetModifierGroupByIdAsync(int id)
    {
        try
        {
            return await _context.Modifiergroups
                .Include(mg => mg.Modifiergroupmodifiers)
                .ThenInclude(mgm => mgm.Modifier)
                .FirstOrDefaultAsync(mg => mg.Id == id);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to retrieve modifier group from database", ex);
        }
    }

    public async Task<bool> UpdateModifierGroup(ModifierGroupViewModel model)
    {
        try
        {
            Modifiergroup? existingGroup = await _context.Modifiergroups
                .Include(mg => mg.Modifiergroupmodifiers)
                .FirstOrDefaultAsync(mg => mg.Id == model.Id);

            if (existingGroup != null)
            {
                existingGroup.Name = model.Name;
                existingGroup.Description = model.Description;

                List<int>? existingModifierIds = existingGroup.Modifiergroupmodifiers
                    .Select(m => m.Modifierid)
                    .ToList();

                List<int>? removeModifierIds = existingModifierIds.Except(model.ModifierIds).ToList();
                List<int>? newModifierIds = model.ModifierIds.Except(existingModifierIds).ToList();

                foreach (int modifierId in newModifierIds)
                {
                    existingGroup.Modifiergroupmodifiers.Add(new Modifiergroupmodifier
                    {
                        Modifiergroupid = model.Id,
                        Modifierid = modifierId
                    });
                }

                foreach (Modifiergroupmodifier? modifier in existingGroup.Modifiergroupmodifiers.Where(m => removeModifierIds.Contains(m.Modifierid)))
                {
                    modifier.Isdeleted = true;
                }

                return await _context.SaveChangesAsync() > 0;
            }

            return false;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating the modifier group", ex);
        }
    }


    // public async Task<Modifiergroup?> GetModifierGroupByIdAsync(int id)
    // {
    //     try
    //     {
    //         return await _context.Modifiergroups.FirstOrDefaultAsync(c => c.Id == id);
    //     }
    //     catch (Exception ex)
    //     {
    //         throw new Exception("An error occurred while retrieving the modifier group", ex);
    //     }
    // }


    public async Task<bool> AddModifierAsync(Modifier modifier, List<int> modifierGroupIds)
    {
        try
        {
            _context.Modifiers.Add(modifier);
            await _context.SaveChangesAsync();

            if (modifierGroupIds != null && modifierGroupIds.Any())
            {
                var mappings = modifierGroupIds.Select(groupId => new Modifiergroupmodifier
                {
                    Modifierid = modifier.Id,
                    Modifiergroupid = groupId
                });

                await _context.Modifiergroupmodifiers.AddRangeAsync(mappings);
            }

            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while adding the modifier.", ex);
        }
    }

    // Repository Layer
    public async Task<Modifier?> GetModifierById(int id)
    {
        try
        {
            return await _context.Modifiers.FirstOrDefaultAsync(x => x.Id == id);
        }
        catch (Exception ex)
        {
            throw new Exception("Error while fetching Modifier from database.", ex);
        }
    }

    public async Task<List<int>> GetModifierGroupIdsByModifierId(int modifierId)
    {
        try
        {
            return await _context.Modifiergroupmodifiers
                .Where(mgm => mgm.Modifierid == modifierId && !mgm.Isdeleted)
                .Select(mgm => mgm.Modifiergroupid)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error while fetching Modifier Group Ids from database.", ex);
        }
    }

    public async Task<bool> UpdateModifierAsync(Modifier modifier)
    {
        try
        {
            _context.Modifiers.Update(modifier);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating the modifier.", ex);
        }
    }


    public async Task UpdateModifierGroupsAsync(int modifierId, List<int> modifierGroupIds)
    {
        try
        {
            List<Modifiergroupmodifier>? existingMappings = await _context.Modifiergroupmodifiers
                .Where(mgm => mgm.Modifierid == modifierId)
                .ToListAsync();

            List<int>? activeGroupIds = existingMappings
                .Where(mgm => !mgm.Isdeleted)
                .Select(mgm => mgm.Modifiergroupid)
                .ToList();

            List<int>? groupsToRemove = activeGroupIds.Except(modifierGroupIds).ToList();
            List<int>? groupsToAdd = modifierGroupIds.Except(activeGroupIds).ToList();

            foreach (Modifiergroupmodifier? mapping in existingMappings.Where(mgm => groupsToRemove.Contains(mgm.Modifiergroupid)))
            {
                mapping.Isdeleted = true;
            }

            foreach (Modifiergroupmodifier? mapping in existingMappings.Where(mgm => groupsToAdd.Contains(mgm.Modifiergroupid)))
            {
                mapping.Isdeleted = false;
                groupsToAdd.Remove(mapping.Modifiergroupid);
            }

            if (groupsToAdd.Any())
            {
                List<Modifiergroupmodifier>? newMappings = groupsToAdd.Select(groupId => new Modifiergroupmodifier
                {
                    Modifierid = modifierId,
                    Modifiergroupid = groupId,
                    Isdeleted = false
                }).ToList();

                await _context.Modifiergroupmodifiers.AddRangeAsync(newMappings);
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating modifier group mappings.", ex);
        }
    }



    public async Task<Modifier?> GetModifierByIdAsync(int id)
    {
        try
        {
            return await _context.Modifiers
                .FirstOrDefaultAsync(m => m.Id == id);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while fetching the modifier by ID.", ex);
        }
    }


    public async Task<bool> SoftDeleteModifierAsync(int id)
    {
        try
        {
            Modifier? modifier = await _context.Modifiers.FindAsync(id);
            if (modifier == null) return false;

            modifier.Isdeleted = true;

            List<Modifiergroupmodifier> relatedModifierGroups = await _context.Modifiergroupmodifiers
                .Where(mgm => mgm.Modifierid == id)
                .ToListAsync();

            foreach (Modifiergroupmodifier mgm in relatedModifierGroups)
            {
                mgm.Isdeleted = true;
            }

            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while soft deleting the modifier.", ex);
        }
    }


    public async Task<bool> SoftDeleteModifierFromGroupAsync(int modifierId, int groupId)
    {
        try
        {
            Modifiergroupmodifier? mapping = await _context.Modifiergroupmodifiers
                .FirstOrDefaultAsync(mgm => mgm.Modifierid == modifierId && mgm.Modifiergroupid == groupId);

            if (mapping != null)
            {
                mapping.Isdeleted = true;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while soft deleting the modifier from the group.", ex);
        }
    }


    public async Task<bool> SoftDeleteModifiersAsync(List<int> modifierIds, int currentGroupId)
    {
        try
        {
            if (modifierIds == null || !modifierIds.Any())
                return false;

            List<Modifiergroupmodifier>? modifiersToUpdate = await _context.Modifiergroupmodifiers
                .Where(mgm => modifierIds.Contains(mgm.Modifierid) && mgm.Modifiergroupid == currentGroupId)
                .ToListAsync();

            if (!modifiersToUpdate.Any())
                return false;

            foreach (Modifiergroupmodifier? modifier in modifiersToUpdate)
            {
                modifier.Isdeleted = true;
            }

            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while soft deleting modifiers.", ex);
        }
    }


    public async Task<PagedResult<ModifierViewModel>> GetAllModifiersToAddModifierGroupAsync(int pageNumber, int pageSize, string searchTerm = "")
    {
        try
        {
            IQueryable<Modifier> query = _context.Modifiers
                .Where(m => !m.Isdeleted);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(m => m.Name.Contains(searchTerm));
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
                    Description = m.Description
                }).ToListAsync();

            return new PagedResult<ModifierViewModel>(modifiers, pageNumber, pageSize, totalCount);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while fetching modifiers for adding to modifier group.", ex);
        }
    }


    public async Task<ModifierGroupViewModel?> GetModifiersByGroupIdAsync(int modifierGroupId)
    {
        try
        {
            return await _context.Modifiergroups
                .Where(mg => mg.Id == modifierGroupId)
                .Select(mg => new ModifierGroupViewModel
                {
                    Id = mg.Id,
                    Name = mg.Name,
                    AvailableModifiers = _context.Modifiergroupmodifiers
                        .Where(mgm => mgm.Modifiergroupid == modifierGroupId && !mgm.Isdeleted)
                        .Select(mgm => new ModifierViewModel
                        {
                            Id = mgm.Modifier.Id,
                            Name = mgm.Modifier.Name,
                            Price = mgm.Modifier.Price
                        }).ToList()
                })
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving the modifiers for the group.", ex);
        }
    }


    // public async Task<List<int>> GetModifierGroupIdsByModifierId(int modifierId)
    // {
    //     return await _context.Modifiergroupmodifiers
    //         .Where(mgm => mgm.Modifierid == modifierId && !mgm.Isdeleted)
    //         .Select(mgm => mgm.Modifiergroupid)
    //         .ToListAsync();
    // }


    public async Task<ItemModifierGroupViewModel> GetModifierGroupByIdForItem(int groupId)
    {
        try
        {
            Modifiergroup? modifierGroup = await _context.Modifiergroups
                .Include(mg => mg.Modifiergroupmodifiers)
                .ThenInclude(mgm => mgm.Modifier)
                .FirstOrDefaultAsync(mg => mg.Id == groupId);

            if (modifierGroup == null) return null;

            return new ItemModifierGroupViewModel
            {
                Id = modifierGroup.Id,
                Name = modifierGroup.Name,
                AvailableModifiersForItem = modifierGroup.Modifiergroupmodifiers
                    .Where(mgm => !mgm.Isdeleted && !mgm.Modifier.Isdeleted)
                    .Select(mgm => new ModifierViewModel
                    {
                        Id = mgm.Modifier.Id,
                        Name = mgm.Modifier.Name,
                        Price = mgm.Modifier.Price
                    })
                    .ToList(),
                MinQuantity = 0,
                MaxQuantity = 0,
                ModifierCount = modifierGroup.Modifiergroupmodifiers.Count(mgm => !mgm.Isdeleted && !mgm.Modifier.Isdeleted)
            };
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving the modifier group for the item.", ex);
        }
    }


    public async Task<List<MappingMenuItemWithModifier>> GetItemWithModifiersByItemIdAsync(int id)
    {
        try
        {
            return await _context.MappingMenuItemWithModifiers
                .Where(m => m.MenuItemId == id)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while retrieving item modifiers for MenuItemId: {id}", ex);
        }
    }

    public async Task<bool> UpdateItemModifiersAsync(List<MappingMenuItemWithModifier> modifiersToUpdate)
    {
        try
        {
            foreach (MappingMenuItemWithModifier? modifier in modifiersToUpdate)
            {
                MappingMenuItemWithModifier? existingModifier = await _context.MappingMenuItemWithModifiers
                    .FirstOrDefaultAsync(m => m.MenuItemId == modifier.MenuItemId && m.ModifierGroupId == modifier.ModifierGroupId);

                if (existingModifier != null)
                {
                    existingModifier.MinModifierCount = modifier.MinModifierCount;
                    existingModifier.MaxModifierCount = modifier.MaxModifierCount;
                }
            }

            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating item modifiers.", ex);
        }
    }
}







