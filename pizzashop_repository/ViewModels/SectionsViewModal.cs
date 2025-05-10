using System.ComponentModel.DataAnnotations;

namespace pizzashop_repository.ViewModels;

public class SectionsViewModal
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
