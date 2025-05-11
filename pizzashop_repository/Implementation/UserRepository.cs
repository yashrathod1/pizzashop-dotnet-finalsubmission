
using Microsoft.EntityFrameworkCore;
using pizzashop_repository.Database;
using pizzashop_repository.Interface;
using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;

namespace pizzashop_repository.Implementation;

public class UserRepository : IUserRepository
{
    private readonly PizzaShopDbContext _context;

    public UserRepository(PizzaShopDbContext context)
    {
        _context = context;
    }

    public User? GetUserByEmail(string email)
    {
        try
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }
        catch (Exception ex)
        {
            throw new Exception("Error getting user by emial", ex);
        }

    }

    public User? GetUserByUsername(string username)
    {
        try
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }
        catch (Exception ex)
        {
            throw new Exception("Error geting user by name", ex);
        }
    }

    public User? GetUserByResetToken(string token)
    {
        try
        {
            return _context.Users.FirstOrDefault(u => u.PasswordResetToken == token);
        }
        catch (Exception ex)
        {
            throw new Exception("Error getting user", ex);
        }

    }


    public bool UpdateUser(User user)
    {
        try
        {
            _context.Users.Update(user);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed To Update User", ex);
        }
    }

    public async Task<string?> GetUserRole(int roleId)
    {
        try
        {
            return await _context.Roles.Where(r => r.Id == roleId).Select(r => r.Rolename).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to get user role", ex);
        }
    }

    public User? GetUserByEmailAndRole(string email)
    {
        try
        {
            return _context.Users
               .Include(u => u.Role)
               .FirstOrDefault(u => u.Email == email);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to get user email and role", ex);
        }

    }

    public async Task<IQueryable<User>> GetAllUsersWithRolesAsync()
    {
        try
        {
            var users = _context.Users
                .Include(u => u.Role)
                .Where(u => !u.Isdeleted)
                .AsQueryable();

            return await Task.FromResult(users);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to get users from database", ex);
        }
    }

    public User? GetUserById(int id)
    {
        try
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving the user by id.", ex);
        }
    }



    public void SoftDeleteUser(User user)
    {
        try
        {
            user.Isdeleted = true;
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new Exception("Error in Deleting User", ex);
        }
    }

    public List<Role> GetRoles()
    {
        try
        {
            return _context.Roles.ToList();
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving the roles.", ex);
        }
    }


    public Role GetRoleById(int id)
    {
        try
        {
            var role = _context.Roles.FirstOrDefault(r => r.Id == id);
            if (role == null)
            {
                throw new KeyNotFoundException("Role not found.");
            }
            return role;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving the role by ID.", ex);
        }
    }

    public void AddUser(User user)
    {
        try
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while adding the user.", ex);
        }
    }

    public User? GetUserByIdAndRole(int id)
    {
        try
        {
            return _context.Users.Include(u => u.Role).FirstOrDefault(u => u.Id == id);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving the user by ID and role.", ex);
        }
    }


    public async Task<Role?> GetRoleByNameAsync(string roleName)
    {
        try
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Rolename == roleName);
        }
        catch
        {
            throw;
        }
    }

    public async Task<List<Roleandpermission>> GetRolePermissionsByRoleIdAsync(int roleId)
    {
        try
        {
            return await _context.Roleandpermissions.Include(rp => rp.Permission)
                .Where(rp => rp.Roleid == roleId)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving permissions by role ID.", ex);
        }
    }


    public async Task<Roleandpermission?> GetRolePermissionByRoleAndPermissionAsync(string? roleName, int? permissionId)
    {
        try
        {
            return await _context.Roleandpermissions
                .FirstOrDefaultAsync(rp => rp.Role.Rolename == roleName && rp.Permissionid == permissionId);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving role permission by role name and permission id.", ex);
        }
    }

    public async Task UpdateRolePermissionAsync(Roleandpermission rolePermission)
    {
        try
        {
            _context.Roleandpermissions.Update(rolePermission);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating the role permission.", ex);
        }
    }



    public async Task<List<Order>> GetOrdersInRangeAsync(DateTime start, DateTime end)
    {
        return await _context.Orders
            .Where(o => o.Createdat >= start && o.Createdat < end)
            .ToListAsync();
    }

    public async Task<List<OrderItemsMapping>> GetServedItemsAsync(DateTime start, DateTime end)
    {
        return await _context.OrderItemsMappings
            .Include(od => od.Order)
            .Where(od => od.Order != null && od.Order.Createdat >= start && od.Order.Createdat < end)
            .ToListAsync();
    }

    public async Task<List<(DateTime Date, int Count)>> GetDailyCustomerCountsAsync(DateTime start, DateTime end)
    {
        return await _context.Customers
            .Where(c => c.Createdat >= start && c.Createdat < end)
            .GroupBy(c => c.Createdat.Date)
            .Select(g => new ValueTuple<DateTime, int>(g.Key, g.Count()))
            .ToListAsync();
    }

    public async Task<List<TopItem>> GetTopItemsAsync(DateTime start, DateTime end)
    {
        return await _context.OrderItemsMappings.Include(od => od.Order)
            .Where(od => od.Order.Createdat >= start && od.Order.Createdat < end  && od.Order.Status != "Cancalled")
            .GroupBy(od => od.ItemName)
            .OrderByDescending(g => g.Sum(od => od.Quantity))
            .Take(5)
            .Select(g => new TopItem
            {
                Name = g.Key,
                OrderCount = g.Sum(od => od.Quantity),
                ImageUrl = "/images/dining-menu.png"
            })
            .ToListAsync();
    }

    public async Task<List<TopItem>> GetLeastItemsAsync(DateTime start, DateTime end)
    {
        return await _context.OrderItemsMappings.Include(od => od.Order)
            .Where(od => od.Order.Createdat >= start && od.Order.Createdat < end  && od.Order.Status != "Cancalled")
            .GroupBy(od => od.ItemName)
            .OrderBy(g => g.Sum(od => od.Quantity))
            .Take(5)
            .Select(g => new TopItem
            {
                Name = g.Key,
                OrderCount = g.Sum(od => od.Quantity),
                ImageUrl = "/images/dining-menu.png"
            })
            .ToListAsync();
    }

    public async Task<List<(DateTime Createdat, DateTime? Servingtime)>> GetServedOrdersAsync(DateTime start, DateTime end)
    {
        return await _context.Orders
            .Where(o => o.Servingtime != null && o.Createdat >= start && o.Createdat < end)
            .Select(o => new ValueTuple<DateTime, DateTime?>(o.Createdat, o.Servingtime))
            .ToListAsync();
    }

    public async Task<int> GetWaitingCountAsync(DateTime start, DateTime end)
    {
        return await _context.WaitingTokens
            .Where(w => !w.IsAssign && !w.Isdeleted && w.Createdat >= start && w.Createdat < end)
            .CountAsync();
    }

    public async Task<int> GetNewCustomerCountAsync(DateTime start, DateTime end)
    {
        return await _context.Customers
            .Where(c => c.Createdat >= start && c.Createdat < end)
            .CountAsync();
    }

}
