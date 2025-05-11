using System.ComponentModel.DataAnnotations;

namespace pizzashop_repository.ViewModels;

public class CategoryViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(20)]
    [RegularExpression(@"^\S[A-Za-z\s]{0,18}\S$", ErrorMessage = "Invalid Name Format")]
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
