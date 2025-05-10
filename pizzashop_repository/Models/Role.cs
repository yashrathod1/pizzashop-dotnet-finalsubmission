using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class Role
{
    public int Id { get; set; }

    public string Rolename { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Roleandpermission> Roleandpermissions { get; } = new List<Roleandpermission>();

    public virtual ICollection<User> Users { get; } = new List<User>();
}
