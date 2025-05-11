
using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;

namespace pizzashop_service.Interface;

public interface IUserService
{
    User? GetUserByEmail(string email);

    User? GetUserByUsername(string username);

    User? AuthenicateUser(string email, string password);
    Task<string> GenerateJwttoken(string email, int roleId);

    bool ResetPassword(string email, string newPassword, string confirmPassword, out string message);

    string ExtractEmailFromToken(string token);

    string GeneratePasswordResetToken(string email);
    ProfileViewModel? GetUserProfile(string email);

    bool UpdateUserProfile(string email, ProfileViewModel model);

    string? ChangePassword(string email, ChangePasswordViewModel model);

    Task<PagedResult<UserTableViewModel>> GetUsersAsync(int pageNumber, int pageSize, string sortBy, string sortOrder, string searchTerm = "");
    bool DeleteUser(int id);

    List<Role> GetRoles();

    Task<bool> AddUser(AddUserViewModel model);

    EditUserViewModel GetUserForEdit(int id);

    Task<bool> EditUser(int id, EditUserViewModel model);

    Task<List<RolePermissionViewModel>> GetPermissionsByRoleAsync(string roleName);

    Task<bool> UpdateRolePermissionsAsync(List<RolePermissionViewModel> permissions);

    Task<DashboardViewModel> GetDashboardDataAsync(string filter,DateTime? customStartDate = null, DateTime? customEndDate = null);

}
