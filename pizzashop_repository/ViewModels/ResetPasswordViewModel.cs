using System.ComponentModel.DataAnnotations;

namespace pizzashop_repository.ViewModels;

public class ResetPasswordViewModel
{
    public string? Email { get; set; }

    [Required(ErrorMessage = "New password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string NewPassword { get; set; } = null!;

    [Required(ErrorMessage = "Confirm password is required")]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = null!;

    public string? Token { get; set; }
}
