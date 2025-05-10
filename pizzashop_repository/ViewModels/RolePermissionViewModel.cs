namespace pizzashop_repository.ViewModels;

public class RolePermissionViewModel
{
    public string? Roleid { get; set; }

    public int? Permissionid { get; set; }

    public string? PermissionName  { get; set; }

    public bool Canview { get; set; }

    public bool CanaddEdit { get; set; }

    public bool Candelete { get; set; }
}
