namespace pizzashop_repository.ViewModels;


public class MenuAppModifierDetailViewModel
{   
    public int ItemId {get; set;}
    public string ItemName { get; set; } = null!;

    public int ItemQuantity { get; set; }

    public List<MenuAppItemModifierGroupViewModel> ModifierGroups { get; set; } = new();
}

public class MenuAppItemModifierGroupViewModel
{   
    public int ModifierGroupId { get; set; }
    public string ModifierGroupName {get; set;} = null!;

    public int MinQuantity { get; set;}

    public int MaxQuantity { get; set;}

    public List<MenuAppItemModifiersViewModel> Modifiers = new List<MenuAppItemModifiersViewModel>();


}
public class MenuAppItemModifiersViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Amount { get; set;} 

    public int Quantity { get; set; }
}