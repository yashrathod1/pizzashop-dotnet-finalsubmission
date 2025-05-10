using System.ComponentModel.DataAnnotations;

namespace pizzashop_repository.ViewModels;

public class ChangePasswordViewModel
{
     public string? Email { get; set; }

    [Required(ErrorMessage = "Current password is required.")]
    public string? CurrentPassword { get; set; }

    [Required(ErrorMessage = "New password is required.")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
    ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
    public string? NewPassword { get; set; }

    [Required(ErrorMessage = "Confirm password is required.")]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string? ConfirmPassword { get; set; }
}
