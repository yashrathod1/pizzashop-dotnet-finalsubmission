using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace pizzashop_repository.ViewModels
{
    public class EditUserViewModel
    {
        public int id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters")]
        [RegularExpression(@"^[a-zA-Z]+(?: [a-zA-Z]+)*$", ErrorMessage = "The Firstname is not valid")]
        public string Firstname { get; set; } = null!;

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters")]
        [RegularExpression(@"^[a-zA-Z]+(?: [a-zA-Z]+)*$", ErrorMessage = "The Lastname is not valid")]
        public string Lastname { get; set; } = null!;

        [Required(ErrorMessage = "Username is required")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Username must be between 5 and 20 characters")]
        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Role ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Role ID")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public int? CountryId { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public int? StateId { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public int? CityId { get; set; }

        [Required(ErrorMessage = "Zipcode is required")]
        [RegularExpression(@"^\d{5,6}$", ErrorMessage = "Zipcode must be 5 or 6 digits")]
        public string Zipcode { get; set; } = null!;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } = null!;

        public IFormFile? ProfileImage { get; set; }

        public string? ProfileImagePath { get; set; }

        public string? RemoveProfileImg {get; set;}
    }
}
