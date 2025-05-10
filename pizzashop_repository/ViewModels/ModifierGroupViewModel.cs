using System.ComponentModel.DataAnnotations;

namespace pizzashop_repository.ViewModels;

public class ModifierGroupViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public List<int>? ModifierIds { get; set; }

    public List<ModifierViewModel> AvailableModifiers { get; set; } = new List<ModifierViewModel>();
}

