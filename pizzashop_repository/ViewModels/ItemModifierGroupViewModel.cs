namespace pizzashop_repository.ViewModels;

public class ItemModifierGroupViewModel
{
  public int GroupId { get; set; }

  public int Id { get; set; }

  public string? Name { get; set; }

  public List<ModifierViewModel> AvailableModifiersForItem { get; set; } = new List<ModifierViewModel>();


  public int MinQuantity { get; set; }


  public int MaxQuantity { get; set; }

  public int ModifierCount { get; set; }
}
