using Microsoft.AspNetCore.Http;

namespace pizzashop_repository.ViewModels;

public class UserTableViewModel
{   

    public int Id { get; set; } 

    public string Email { get; set; } = null!;
    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Username { get; set; } = null!;


    public string Phone { get; set; } = null!;

    public string? Rolename { get; set; }

    public int? CountryId { get; set; }
    public int? StateId { get; set; }
    public int? CityId { get; set; }

    public string? Status { get; set; }

    public string Zipcode { get; set; } = null!;

    public string Address { get; set; } = null!;

    public IFormFile? ProfileImage { get; set; }

    public string? ProfileImagePath { get; set; }

    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
}
