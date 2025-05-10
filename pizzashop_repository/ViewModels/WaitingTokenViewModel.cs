using System.ComponentModel.DataAnnotations;

namespace pizzashop_repository.ViewModels
{
    public class WaitingTokenViewModel
    {   
        public int Id { get; set;}

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

        [Required(ErrorMessage = "Section is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid section.")]
        public int SectionId { get; set; }
    }
}
