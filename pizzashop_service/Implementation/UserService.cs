using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using pizzashop_repository.Database;
using pizzashop_repository.Interface;
using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;
using pizzashop_service.Interface;

namespace pizzashop_service.Implementation;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IUserRepository userRepository, IJwtService jwtService, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _httpContextAccessor = httpContextAccessor;

    }

    public User? GetUserByEmail(string email)
    {
        try
        {
            return _userRepository.GetUserByEmail(email);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving the user by email.", ex);
        }
    }


    public User? GetUserByUsername(string username)
    {
        try
        {
            return _userRepository.GetUserByUsername(username);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving the user by username.", ex);
        }
    }

    public User? AuthenicateUser(string email, string password)
    {
        try
        {
            User? user = _userRepository.GetUserByEmail(email);

            if (user == null)
            {
                return null;
            }

            if (!user.Password.StartsWith("$2a$") && !user.Password.StartsWith("$2b$") && !user.Password.StartsWith("$2y$"))
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
                user.Password = hashedPassword;
                _userRepository.UpdateUser(user);
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null;
            }

            return user;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while authenticating the user.", ex);
        }
    }

    public async Task<string> GenerateJwttoken(string email, int roleId)
    {
        try
        {
            return await _jwtService.GenerateJwtToken(email, roleId);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while generating the JWT token.", ex);
        }
    }


    public string GeneratePasswordResetToken(string email)
    {
        try
        {
            User? user = _userRepository.GetUserByEmail(email);
            if (user == null) return null;

            user.PasswordResetToken = Guid.NewGuid().ToString();
            user.Resettokenexpiry = DateTime.UtcNow.AddHours(1);

            _userRepository.UpdateUser(user);

            return user.PasswordResetToken;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while generating the password reset token.", ex);
        }
    }



    public bool ResetPassword(string token, string newPassword, string confirmPassword, out string message)
    {
        try
        {
            if (newPassword != confirmPassword)
            {
                message = "The new password and confirmation password do not match.";
                return false;
            }

            User? user = _userRepository.GetUserByResetToken(token);

            if (user == null || user.Resettokenexpiry < DateTime.UtcNow)
            {
                message = "Invalid or expired reset token.";
                return false;
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.PasswordResetToken = null;
            user.Resettokenexpiry = null;

            _userRepository.UpdateUser(user);

            message = "Password has been successfully updated.";
            return true;
        }
        catch (Exception)
        {
            message = "An error occurred while resetting the password.";
            return false;
        }
    }

    public string ExtractEmailFromToken(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
                return string.Empty;

            var handler = new JwtSecurityTokenHandler();
            var authToken = handler.ReadJwtToken(token);
            return authToken.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Email)?.Value ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    public ProfileViewModel? GetUserProfile(string email)
    {
        try
        {
            if (string.IsNullOrEmpty(email))
                return null;

            User? user = _userRepository.GetUserByEmailAndRole(email);
            if (user == null)
                return null;

            return new ProfileViewModel
            {
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Username = user.Username,
                Email = user.Email,
                Rolename = user.Role.Rolename,
                CountryId = user.Countryid,
                StateId = user.Stateid,
                CityId = user.Cityid,
                Phone = user.Phone,
                Address = user.Address,
                Zipcode = user.Zipcode,
                ProfileImagePath = user.Profileimagepath
            };
        }
        catch
        {
            return null;
        }
    }

    public bool UpdateUserProfile(string email, ProfileViewModel model)
    {
        try
        {
            User? user = _userRepository.GetUserByEmail(email);
            if (user == null) return false;

            user.Firstname = model.Firstname;
            user.Lastname = model.Lastname;
            user.Username = model.Username;
            user.Phone = model.Phone;
            user.Countryid = model.CountryId;
            user.Stateid = model.StateId;
            user.Cityid = model.CityId;
            user.Address = model.Address;
            user.Zipcode = model.Zipcode;

            if (model.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/users");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (FileStream? fileStream = new(filePath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }

                if (!string.IsNullOrEmpty(user.Profileimagepath))
                {
                    string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.Profileimagepath.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                user.Profileimagepath = "/images/users/" + uniqueFileName;
            }

            model.ProfileImagePath = user.Profileimagepath;

            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                context.Response.Cookies.Append("Username", model.Username);
                context.Response.Cookies.Append("ProfileImgPath", model.ProfileImagePath);
            }


            return _userRepository.UpdateUser(user);
        }
        catch
        {
            return false;
        }
    }

    public string ChangePassword(string email, ChangePasswordViewModel model)
    {
        try
        {
            User? user = _userRepository.GetUserByEmail(email);

            if (user == null)
            {
                return "UserNotFound";
            }

            if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.Password))
            {
                return "IncorrectPassword";
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            _userRepository.UpdateUser(user);

            return "Success";
        }
        catch (Exception ex)
        {
            throw new Exception("change password error occurs", ex);
        }
    }


    public async Task<PagedResult<UserTableViewModel>> GetUsersAsync(int pageNumber, int pageSize, string sortBy, string sortOrder, string searchTerm = "")
    {
        try
        {
            var query = await _userRepository.GetAllUsersWithRolesAsync();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(i =>
                    i.Firstname.ToLower().Contains(searchTerm.ToLower()) ||
                    i.Lastname.ToLower().Contains(searchTerm.ToLower()));
            }

            query = sortBy switch
            {
                "Name" => sortOrder == "asc"
                    ? query.OrderBy(u => u.Firstname)
                    : query.OrderByDescending(u => u.Firstname),

                "Role" => sortOrder == "asc"
                    ? query.OrderBy(u => u.Role.Rolename)
                    : query.OrderByDescending(u => u.Role.Rolename),

                _ => query.OrderBy(u => u.Id)
            };

            int totalCount = await query.CountAsync();


            List<UserTableViewModel> users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new UserTableViewModel
                {
                    Id = t.Id,
                    Firstname = t.Firstname,
                    Lastname = t.Lastname,
                    Email = t.Email,
                    Phone = t.Phone,
                    Rolename = t.Role.Rolename,
                    Status = t.Status,
                    ProfileImagePath = t.Profileimagepath
                })
                .ToListAsync();

            return new PagedResult<UserTableViewModel>(users, pageNumber, pageSize, totalCount);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to fetch paginated users", ex);
        }
    }


    public bool DeleteUser(int id)
    {
        try
        {
            User? user = _userRepository.GetUserById(id);
            if (user == null)
            {
                return false;
            }

            _userRepository.SoftDeleteUser(user);
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error occurred while deleting user with ID: {id}", ex);
        }
    }


    public List<Role> GetRoles()
    {
        try
        {
            return _userRepository.GetRoles();
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred while retrieving roles.", ex);
        }
    }


    public async Task<bool> AddUser(AddUserViewModel model)
    {
        try
        {
            Role? role = _userRepository.GetRoleById(model.RoleId);
            if (role == null)
            {
                return false;
            }

            User? user = new User
            {
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                Email = model.Email,
                Username = model.Username,
                Phone = model.Phone,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Roleid = model.RoleId,
                Profileimagepath = model.ProfileImagePath,
                Countryid = model.CountryId,
                Stateid = model.StateId,
                Cityid = model.CityId,
                Address = model.Address,
                Zipcode = model.Zipcode,
                Createdby = role.Rolename
            };

            if (model.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/users");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (FileStream? fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(fileStream);
                }

                user.Profileimagepath = "/images/users/" + uniqueFileName;
            }

            _userRepository.AddUser(user);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred while adding a user.", ex);
        }
    }

    public EditUserViewModel GetUserForEdit(int id)
    {
        try
        {
            User? user = _userRepository.GetUserById(id);

            if (user == null) return null;

            return new EditUserViewModel
            {
                id = user.Id,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email,
                Username = user.Username,
                Phone = user.Phone,
                Status = user.Status,
                RoleId = user.Roleid,
                CountryId = user.Countryid,
                StateId = user.Stateid,
                CityId = user.Cityid,
                Address = user.Address,
                Zipcode = user.Zipcode,
                ProfileImagePath = user.Profileimagepath
            };
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred while fetching user for edit.", ex);
        }
    }


    public async Task<bool> EditUser(int id, EditUserViewModel model)
    {
        try
        {
            User? user = _userRepository.GetUserById(id);
            if (user == null) return false;

            bool hasChanges =
                user.Firstname != model.Firstname ||
                user.Lastname != model.Lastname ||
                user.Email != model.Email ||
                user.Username != model.Username ||
                user.Phone != model.Phone ||
                user.Status != model.Status ||
                user.Roleid != model.RoleId ||
                user.Countryid != model.CountryId ||
                user.Stateid != model.StateId ||
                user.Cityid != model.CityId ||
                user.Address != model.Address ||
                user.Zipcode != model.Zipcode ||
                (model.ProfileImage != null && model.ProfileImage.Length > 0) ||
                (model.RemoveProfileImg == "true");

            if (!hasChanges)
            {
                return false;
            }

            user.Firstname = model.Firstname;
            user.Lastname = model.Lastname;
            user.Email = model.Email;
            user.Username = model.Username;
            user.Phone = model.Phone;
            user.Status = model.Status;
            user.Roleid = model.RoleId;
            user.Countryid = model.CountryId;
            user.Stateid = model.StateId;
            user.Cityid = model.CityId;
            user.Address = model.Address;
            user.Zipcode = model.Zipcode;

            if (model.RemoveProfileImg == "true")
            {
                if (!string.IsNullOrEmpty(user.Profileimagepath))
                {
                    string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.Profileimagepath.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                    user.Profileimagepath = null;
                }
            }

            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/users");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (FileStream? fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(fileStream);
                }

                if (!string.IsNullOrEmpty(user.Profileimagepath))
                {
                    string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.Profileimagepath.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                user.Profileimagepath = "/images/users/" + uniqueFileName;
            }

            _userRepository.UpdateUser(user);
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred while editing the user profile.", ex);
        }
    }


    public async Task<List<RolePermissionViewModel>> GetPermissionsByRoleAsync(string roleName)
    {
        try
        {
            var role = await _userRepository.GetRoleByNameAsync(roleName);
            if (role == null)
            {
                throw new Exception($"Role with name {roleName} not found.");
            }

            var rolePermissions = await _userRepository.GetRolePermissionsByRoleIdAsync(role.Id);

            var permissions = rolePermissions.Select(rp => new RolePermissionViewModel
            {
                Permissionid = rp.Permissionid,
                PermissionName = rp.Permission.Permissiomname,
                Canview = rp.Canview,
                CanaddEdit = rp.CanaddEdit,
                Candelete = rp.Candelete
            }).ToList();

            return permissions;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> UpdateRolePermissionsAsync(List<RolePermissionViewModel> permissions)
    {
        try
        {
            foreach (var permission in permissions)
            {
                var rolePermission = await _userRepository.GetRolePermissionByRoleAndPermissionAsync(permission.Roleid, permission.Permissionid);

                if (rolePermission != null)
                {
                    rolePermission.Canview = permission.Canview;
                    rolePermission.CanaddEdit = permission.CanaddEdit;
                    rolePermission.Candelete = permission.Candelete;

                    await _userRepository.UpdateRolePermissionAsync(rolePermission);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating role permissions.", ex);
        }
    }

    public async Task<DashboardViewModel> GetDashboardDataAsync(string filter,DateTime? customStartDate = null, DateTime? customEndDate = null)
    {
        DateTime startDate, endDate;
        var today = DateTime.Today;

        if (filter == "Custom" && customStartDate.HasValue && customEndDate.HasValue)
        {
            startDate = customStartDate.Value.Date;
            endDate = customEndDate.Value.Date.AddDays(1); 
        }
        else
        {
            switch (filter)
            {
                case "Today":
                    startDate = today;
                    endDate = today.AddDays(1);
                    break;
                case "Last 7 Days":
                    startDate = today.AddDays(-6);
                    endDate = today.AddDays(1);
                    break;
                case "Last 30 Days":
                    startDate = today.AddDays(-29);
                    endDate = today.AddDays(1);
                    break;
                case "Current Month":
                default:
                    startDate = new DateTime(today.Year, today.Month, 1);
                    endDate = startDate.AddMonths(1);
                    break;
            }
        }

        var ordersInRange = await _userRepository.GetOrdersInRangeAsync(startDate, endDate);
        var servedItems = await _userRepository.GetServedItemsAsync(startDate, endDate);
        var dailyCustomerCounts = await _userRepository.GetDailyCustomerCountsAsync(startDate, endDate);
        var topItems = await _userRepository.GetTopItemsAsync(startDate, endDate);
        var leastItems = await _userRepository.GetLeastItemsAsync(startDate, endDate);
        var servedOrders = await _userRepository.GetServedOrdersAsync(startDate, endDate);
        var waitingCount = await _userRepository.GetWaitingCountAsync(startDate, endDate);
        var newCustomer = await _userRepository.GetNewCustomerCountAsync(startDate, endDate);

        // Aggregations
        var totalSales = ordersInRange.Where(o => o.Status != "Cancalled").Sum(o => o.Totalamount);
        var totalOrders = ordersInRange.Count;
        var avgOrderValue = Math.Round(totalOrders > 0 ? totalSales / totalOrders : 0, 2);

        var revenueChart = ordersInRange
            .GroupBy(o => o.Createdat.Date)
            .Select(g => new ChartDataPoint
            {
                Label = g.Key.ToString("MMM dd"),
                Value = g.Sum(o => o.Totalamount)
            })
            .OrderBy(g => g.Label)
            .ToList();

        var cumulativeList = new List<ChartDataPoint>();
        int runningTotal = 0;
        foreach (var (Date, Count) in dailyCustomerCounts)
        {
            runningTotal += Count;
            cumulativeList.Add(new ChartDataPoint
            {
                Label = Date.ToString("MMM dd"),
                Value = runningTotal
            });
        }

        var waitTimeMinutes = servedOrders
            .Select(o => (o.Servingtime.Value - o.Createdat).TotalMinutes)
            .ToList();

        var avgWaitTime = waitTimeMinutes.Any() ? Math.Round(waitTimeMinutes.Average(), 1) : 0;

        return new DashboardViewModel
        {
            TotalSales = totalSales,
            TotalOrders = totalOrders,
            AverageOrderValue = avgOrderValue,
            RevenueChartData = revenueChart,
            CustomerGrowthData = cumulativeList,
            TopSellingItems = topItems,
            LeastSellingItems = leastItems,
            AverageWaitingTime = avgWaitTime,
            WaitingListCount = waitingCount,
            NewCustomer = newCustomer
        };
    }


}
