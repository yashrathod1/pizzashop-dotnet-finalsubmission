using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;

namespace pizzashop_repository.Interface;

public interface IUserRepository
{
    User? GetUserByEmail(string email);

    User? GetUserByUsername(string username);

    User GetUserByResetToken(string token);

    bool UpdateUser(User user);

    Task<string?> GetUserRole(int roleId);


    User? GetUserByEmailAndRole(string email);

    Task<IQueryable<User>> GetAllUsersWithRolesAsync();
    User GetUserById(int id);

    void SoftDeleteUser(User user);

    List<Role> GetRoles();
    Role GetRoleById(int id);

    void AddUser(User user);

    User? GetUserByIdAndRole(int id);

    Task<Role?> GetRoleByNameAsync(string roleName);

    Task<List<Roleandpermission>> GetRolePermissionsByRoleIdAsync(int roleId);

    Task<Roleandpermission?> GetRolePermissionByRoleAndPermissionAsync(string? roleName, int? permissionId);

    Task UpdateRolePermissionAsync(Roleandpermission rolePermission);

    Task<List<Order>> GetOrdersInRangeAsync(DateTime start, DateTime end);

    Task<List<OrderItemsMapping>> GetServedItemsAsync(DateTime start, DateTime end);

    Task<List<(DateTime Date, int Count)>> GetDailyCustomerCountsAsync(DateTime start, DateTime end);

    Task<List<TopItem>> GetTopItemsAsync(DateTime start, DateTime end);

    Task<List<TopItem>> GetLeastItemsAsync(DateTime start, DateTime end);

    Task<List<(DateTime Createdat, DateTime? Servingtime)>> GetServedOrdersAsync(DateTime start, DateTime end);

    Task<int> GetWaitingCountAsync(DateTime start, DateTime end);

    Task<int> GetNewCustomerCountAsync(DateTime start, DateTime end);

}


