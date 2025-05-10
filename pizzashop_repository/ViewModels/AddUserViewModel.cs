using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace pizzashop_repository.ViewModels
{
    public class AddUserViewModel
    {
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z]+(?: [a-zA-Z]+)*$", ErrorMessage = "The Firstname is not valid")]
        public string Firstname { get; set; } = null!;

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z]+(?: [a-zA-Z]+)*$", ErrorMessage = "The Lastname is not valid")]
        public string Lastname { get; set; } = null!;

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Username must be between 3 and 20 characters.")]
        [RegularExpression(@"^[a-zA-Z]+(?: [a-zA-Z]+)*$", ErrorMessage = "The Username is not valid")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "password is required.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must be at least 8 characters, include uppercase, lowercase, a number, and a special character.")]
        public string Password { get; set; } = null!;


        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Please select a role.")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public int? CountryId { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public int? StateId { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public int? CityId { get; set; }

        [Required(ErrorMessage = "Zipcode is required.")]
        [RegularExpression(@"^\d{5,6}$", ErrorMessage = "Zipcode must be 5 or 6 digits.")]
        public string Zipcode { get; set; } = null!;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; } = null!;

        public string? Createdby { get; set; }

        public IFormFile? ProfileImage { get; set; }

        public string? ProfileImagePath { get; set; }
    }
}
