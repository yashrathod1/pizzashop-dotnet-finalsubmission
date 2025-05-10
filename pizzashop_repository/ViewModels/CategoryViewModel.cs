using System.ComponentModel.DataAnnotations;

namespace pizzashop_repository.ViewModels;

public class CategoryViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(20)]
    [RegularExpression(@"^\S[A-Za-z\s]{0,18}\S$", ErrorMessage = "name must start and end with a letter, contain only letters and spaces, and have a maximum length of 20 And Min Length is 2")]
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
