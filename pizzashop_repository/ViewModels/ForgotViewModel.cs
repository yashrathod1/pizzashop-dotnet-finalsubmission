using System.ComponentModel.DataAnnotations;

namespace pizzashop_repository.ViewModels;

public class ForgotViewModel
{
    [Required]
    public string Email { get; set; } = null!;
}
