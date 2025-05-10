namespace pizzashop_repository.ViewModels;

public class RemoveModifierFromGroupViewModel
{
    public int ModifierId { get; set; }
    public int GroupId { get; set; }

    public List<int>? modifierIds { get; set; }
}
