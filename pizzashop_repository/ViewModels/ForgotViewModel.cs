using System.ComponentModel.DataAnnotations;

namespace pizzashop_repository.ViewModels;

public class ForgotViewModel
{
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = null!;
}
