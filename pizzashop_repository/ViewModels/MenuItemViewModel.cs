using System.ComponentModel.DataAnnotations;

namespace pizzashop_repository.ViewModels;

public class MenuItemViewModel
{
    public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
    public List<ItemViewModel> Items { get; set; } = new List<ItemViewModel>();

    public List<ModifierGroupViewModel> ModifierGroup { get; set; } = new List<ModifierGroupViewModel>();

    public List<ModifierViewModel> Modifiers { get; set; } = new List<ModifierViewModel>();

    [Required]
    public CategoryViewModel Category { get; set; } = new CategoryViewModel();

    [Required]
    public ItemViewModel item { get; set; } = new ItemViewModel();

     [Required]
    public ModifierViewModel modifier { get; set; } = new ModifierViewModel();

    [Required]
    public ModifierGroupViewModel ModifierGroups { get; set; } = new ModifierGroupViewModel();
    public PagedResult<ModifierViewModel>? Modifierss { get; set; }

}
