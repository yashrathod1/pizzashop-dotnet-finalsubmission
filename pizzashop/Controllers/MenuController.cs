using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;
using pizzashop_service.Interface;

namespace pizzashop.Controllers;

public class MenuController : Controller
{
    private readonly IMenuService _menuService;


    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [CustomAuthorize("Menu", "CanView")]
    [HttpGet]
    public async Task<IActionResult> Menu()
    {
        ViewBag.ActiveNav = "Menu";
        RolePermissionViewModel? permission = await PermissionHelper.GetPermissionsAsync(HttpContext, "Menu");
        ViewBag.Permissions = permission;

        // List<CategoryViewModel>? categories = await _menuService.GetCategoriesAsync();

        // categories = categories.OrderBy(c => c.Id).ToList();

        // List<ItemViewModel>? items = await _menuService.GetItemsAsync();

        // items = items.OrderBy(i => i.Id).ToList();

        List<ModifierGroupViewModel>? modifierGroup = await _menuService.GetModifierGroupAsync();

        modifierGroup = modifierGroup.OrderBy(mg => mg.Id).ToList();

        // List<ModifierViewModel>? modifiers = await _menuService.GetModifiersAsync();

        // modifiers = modifiers.OrderBy(m => m.Id).ToList();

        // Task<List<ModifierViewModel>>? pageModifiers = _menuService.GetModifiersAsync();

        MenuItemViewModel? viewmodel = new MenuItemViewModel
        {
            // Categories = categories,
            // Items = items,
            ModifierGroup = modifierGroup
            // Modifiers = modifiers,

        };

        return View(viewmodel);

    }

    [CustomAuthorize("Menu", "CanView")]
    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        RolePermissionViewModel? permission = await PermissionHelper.GetPermissionsAsync(HttpContext, "Menu");
        ViewBag.Permissions = permission;
        List<CategoryViewModel>? categories = await _menuService.GetCategoriesAsync();

        categories = categories.OrderBy(c => c.Id).ToList();
        return PartialView("_CategoryListPartial", categories);
    }

    public async Task<IActionResult> GetCategoriesForModal()
    {
        List<CategoryViewModel>? categories = await _menuService.GetCategoriesAsync();
        return Json(categories);
    }

    public async Task<IActionResult> GetModifierGroupForModal()
    {
        List<ModifierGroupViewModel>? modifiergroup = await _menuService.GetModifierGroupAsync();
        return Json(modifiergroup);
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory([FromBody] CategoryViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Name))
        {
            return Json(new { success = false, message = "Category name is required" });
        }

        CategoryViewModel? existingCategory = await _menuService.GetCategoryByNameAsync(model.Name);
        if (existingCategory != null)
        {
            return Json(new { success = false, message = "Category name already exists" });
        }

        string? userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "Unknown";
        var category = await _menuService.AddCategoryAsync(model.Name, model.Description, userRole);
        return Json(new { success = true, message = "Category added successfully" });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCategory([FromBody] CategoryViewModel model)
    {
        if (model == null || model.Id == 0)
        {
            return Json(new { success = false, message = "Invalid Category Data." });
        }

        CategoryViewModel? duplicateCategory = await _menuService.GetCategoryByNameAsync(model.Name);
        if (duplicateCategory != null && duplicateCategory.Id != model.Id)
        {
            return Json(new { success = false, message = "Category name already exists." });
        }

        CategoryViewModel? existingCategory = await _menuService.GetCategoryByIdAsync(model.Id);
        if (existingCategory == null)
        {
            return Json(new { success = false, message = "Category not found." });
        }

        if (existingCategory.Name == model.Name && existingCategory.Description == model.Description)
        {
            return Json(new { success = false, message = "No changes detected." });
        }

        string? updatedBy = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "Unknown";

        var category = new Category
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description
        };

        bool result = await _menuService.UpdateCategoryAsync(category, updatedBy);

        if (result)
        {
            return Ok(new { success = true, message = "Category updated successfully." });
        }
        else
        {
            return BadRequest(new { success = false, message = "Failed to update category." });
        }
    }


    [HttpPost]
    public async Task<IActionResult> DeleteCategory([FromBody] CategoryDeleteViewModel request)
    {
        bool result = await _menuService.SoftDeleteCategoryAsync(request.Id);
        return result ? Ok(new { success = true }) : BadRequest(new { success = false });
    }



    [CustomAuthorize("Menu", "CanView")]
    [HttpGet]
    public async Task<IActionResult> GetItemsByCategory(int categoryId = 1, int pageNumber = 1, int pageSize = 5, string searchTerm = "")
    {
        RolePermissionViewModel? permission = await PermissionHelper.GetPermissionsAsync(HttpContext, "Menu");
        ViewBag.Permissions = permission;
        PagedResult<ItemViewModel>? pagedItems = await _menuService.GetItemsByCategoryAsync(categoryId, pageNumber, pageSize, searchTerm);
        return PartialView("_ItemListPartial", pagedItems);
    }



    [HttpGet]
    public IActionResult GetItemById(int id)
    {
        ItemViewModel? item = _menuService.GetItemById(id);
        if (item == null)
        {
            return NotFound();
        }
        return Json(item);
    }

    [HttpGet]
    public IActionResult LoadModifierGroupsIntoModal(int id)
    {
        ItemViewModel? item = _menuService.GetItemById(id);
        if (item == null)
        {
            return NotFound();
        }
        return PartialView("_ItemModifierWrapperPartial", item.ModifierGroups);
    }


    [HttpPost]
    public async Task<IActionResult> AddMenuItem(ItemViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Loop through all model state errors
            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                foreach (var error in errors)
                {
                    // Log, return, or collect errors as needed
                    Console.WriteLine($"Field: {key}, Error: {error.ErrorMessage}");
                }
            }
            return Json(new { success = false, message = "Invalid data provided." });
        }

        if (model.ItemPhoto != null && model.ItemPhoto.Length > 0)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ItemPhoto.FileName);
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/ItemImage/", fileName);
            using (FileStream? stream = new FileStream(filePath, FileMode.Create))
            {
                model.ItemPhoto.CopyTo(stream);
            }
            model.ItemImagePath = "/images/ItemImage/" + fileName;
        }

        bool result = await _menuService.AddItemAsync(model);

        if (result)
        {
            return Json(new { success = true, message = "Item added successfully." });
        }
        else
        {
            return Json(new { success = false, message = "Failed to add item." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditMenuItem(int id, [FromForm] ItemViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(ModelState);
        }

        if (model.ItemPhoto != null && model.ItemPhoto.Length > 0)
        {
            if (!string.IsNullOrEmpty(model.ItemImagePath))
            {
                string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", model.ItemImagePath.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ItemPhoto.FileName);
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/ItemImage/", fileName);
            using (FileStream? stream = new FileStream(filePath, FileMode.Create))
            {
                model.ItemPhoto.CopyTo(stream);
            }
            model.ItemImagePath = "/images/ItemImage/" + fileName;
        }


        var result = await _menuService.UpdateItemAsync(model, id);
        if (result)
        {
            return Json(new { success = true, message = "Item edited successfully" });
        }
        return Json(new { success = false, message = "No Changes Detected in Item" });
    }


    [HttpPost]
    public async Task<IActionResult> DeleteItem([FromBody] ItemViewModel request)
    {
        bool result = await _menuService.SoftDeleteItemAsync(request.Id);
        return result ? Ok(new { success = true }) : BadRequest(new { success = false });
    }

    [HttpPost]
    public IActionResult SoftDeleteItems([FromBody] List<int> itemIds)
    {
        if (itemIds == null || itemIds.Count == 0)
        {
            return Json(new { success = false, message = "No item Selected" });
        }

        _menuService.SoftDeleteItems(itemIds);
        return Json(new { success = true });
    }


    // modifiers side


    [HttpGet]
    public async Task<IActionResult> GetModifierGroup()
    {
        RolePermissionViewModel? permission = await PermissionHelper.GetPermissionsAsync(HttpContext, "Menu");
        ViewBag.Permissions = permission;
        List<ModifierGroupViewModel>? modifierGroups = await _menuService.GetModifierGroupAsync();

        modifierGroups = modifierGroups.OrderBy(c => c.Id).ToList();
        return PartialView("_ModifierGroupPartial", modifierGroups);
    }

    [HttpGet]
    public async Task<IActionResult> GetModifiersByModifierGroup(int modifierGroupId = 1, int pageNumber = 1, int pageSize = 5, string searchTerm = "")
    {
        RolePermissionViewModel? permission = await PermissionHelper.GetPermissionsAsync(HttpContext, "Menu");
        ViewBag.Permissions = permission;
        PagedResult<ModifierViewModel>? modifiers = await _menuService.GetModifiersByModifierGroupAsync(modifierGroupId, pageNumber, pageSize, searchTerm);
        return PartialView("_ModifierListPartial", modifiers);
    }



    [HttpPost]
    public async Task<IActionResult> AddModifierGroup([FromBody] ModifierGroupViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Invalid data. Please check your input." });
        }

        bool isAdded = await _menuService.AddModifierGroup(model);
        if (isAdded)
        {
            return Json(new { success = true, message = "Modifier Group added successfully!" });
        }
        else
        {
            return Json(new { success = false, message = "A modifier group with this name already exists!" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteModifierGroup([FromBody] ModifierGroupViewModel request)
    {
        bool result = await _menuService.SoftDeleteModifierGroupAsync(request.Id);
        return result ? Ok(new { success = true }) : BadRequest(new { success = false });
    }

    [HttpGet]
    public async Task<IActionResult> GetModifierGroupById(int id)
    {
        ModifierGroupViewModel? model = await _menuService.GetModifierGroupById(id);
        if (model != null)
        {
            return Json(model);
        }
        return Json(null);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateModifierGroup([FromBody] ModifierGroupViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Invalid data. Please check your input." });
        }

        bool isUpdated = await _menuService.UpdateModifierGroup(model);

        if (isUpdated)
        {
            return Json(new { success = true, message = "Modifier Group updated successfully!" });
        }

        return Json(new { success = false, message = "No changes detected or a modifier group with this name already exists!" });
    }


    [HttpPost]
    public async Task<IActionResult> AddModifier([FromBody] ModifierViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Invalid data" });
        }
        if (ModelState.IsValid)
        {
            bool isAdded = await _menuService.AddModifierAsync(model);
            if (isAdded)
            {
                return Json(new { success = true });
            }
        }
        return Json(new { success = false });
    }

    [HttpGet]
    public async Task<IActionResult> GetModifierById(int id)
    {
        ModifierViewModel? modifier = await _menuService.GetModifierById(id);
        if (modifier == null)
        {
            return NotFound();
        }
        return Json(modifier);
    }
    [HttpPost]
    public async Task<IActionResult> EditModifier([FromBody] ModifierViewModel model)
    {
        bool result = await _menuService.UpdateModifierAsync(model);
        if (result)
        {
            return Json(new { success = true });
        }
        return Json(new { success = false, message = "No changes detected." });
    }


    [HttpPost]
    public async Task<IActionResult> SoftDeleteModifierFromGroup([FromBody] RemoveModifierFromGroupViewModel model)
    {
        if (model.ModifierId <= 0 || model.GroupId <= 0)
        {
            return Json(new { success = false, message = "Invalid Modifier or Group ID" });
        }

        bool result = await _menuService.SoftDeleteModifierFromGroupAsync(model.ModifierId, model.GroupId);
        if (result)
        {
            return Json(new { success = true });
        }
        return Json(new { success = false });
    }

    [HttpPost]
    public async Task<IActionResult> SoftDeleteModifiers([FromBody] RemoveModifierFromGroupViewModel model)
    {
        if (model.modifierIds == null || !model.modifierIds.Any())
        {
            return Json(new { success = false, message = "No Modifiers Selected" });
        }

        bool result = await _menuService.SoftDeleteModifiersAsync(model.modifierIds, model.GroupId);

        if (result)
        {
            return Json(new { success = true, message = "Modifiers s deleted successfully" });
        }
        else
        {
            return Json(new { success = false, message = "Failed to  delete modifiers" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllModifiersToModal(int pageNumber = 1, int pageSize = 5, string searchTerm = "")
    {
        PagedResult<ModifierViewModel>? modifiers = await _menuService.GetAllModifiersToAddModifierGroupAsync(pageNumber, pageSize, searchTerm);
        return PartialView("_GetAllModifiersToListPartial", modifiers);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllModifiersToModalForEdit(int pageNumber = 1, int pageSize = 5, string searchTerm = "")
    {
        PagedResult<ModifierViewModel>? modifiers = await _menuService.GetAllModifiersToAddModifierGroupAsync(pageNumber, pageSize, searchTerm);
        return PartialView("_GetAllModifiersToListForEditPartial", modifiers);
    }


    [HttpGet]
    public async Task<IActionResult> GetModifierGroupForItem(int groupId)
    {
        ItemModifierGroupViewModel? modifierGroup = await _menuService.GetModifierGroupByIdForItem(groupId);
        if (modifierGroup == null)
        {
            return NotFound();
        }
        return PartialView("_ItemModifierPartial", modifierGroup);
    }


}

