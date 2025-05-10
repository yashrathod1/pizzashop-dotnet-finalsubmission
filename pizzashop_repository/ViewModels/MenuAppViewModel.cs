using System.ComponentModel.DataAnnotations;

namespace pizzashop_repository.ViewModels;

public class MenuAppViewModel
{
    public List<CategoryViewModel> CategoryList { get; set; } = new();

    public MenuAppCustomerViewModel Customer { get; set; } = new MenuAppCustomerViewModel();
}

public class MenuAppTableSectionViewModel
{
    public string? SectionName { get; set; }

    public List<string> TableName { get; set; } = null!;
}

public class MenuAppCustomerViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [RegularExpression(@"^\S+@\S+\.\S+$", ErrorMessage = "Email cannot contain whitespace.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Name is required.")]
    [MinLength(2, ErrorMessage = "Name must be at least 2 characters.")]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Name cannot be blank or whitespace.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Mobile number is required.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be at least 10 digits.")]
    public string MobileNo { get; set; } = null!;

    [Required(ErrorMessage = "Number of persons is required.")]
    [Range(1, 100, ErrorMessage = "No of Person(s) must be between 1 to 100")]
    public int NoOfPerson { get; set; }

}

public class MenuAppOrderViewModel
{
    public int Id { get; set; }
    public string? OrderComment { get; set; }

    public string? Status { get; set; }
}

public class MenuAppCustomerFeedbackViewModel
{
    public int OrderId { get; set; }
    public int FoodRating { get; set; }
    public int ServiceRating { get; set; }
    public int AmbienceRating { get; set; }
    public string? Comment { get; set; }
}

public class MenuAppItemInstructionViewModel
{
    public int ItemId { get; set; }
    public string? Instruction { get; set; }

    public int OrderId { get; set; }
}