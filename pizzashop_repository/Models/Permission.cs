using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class Permission
{
    public int Id { get; set; }

    public string? Permissiomname { get; set; }

    public virtual ICollection<Roleandpermission> Roleandpermissions { get; } = new List<Roleandpermission>();
}
