using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class Roleandpermission
{
    public int Id { get; set; }

    public int? Roleid { get; set; }

    public int? Permissionid { get; set; }

    public bool Canview { get; set; }

    public bool CanaddEdit { get; set; }

    public bool Candelete { get; set; }

    public bool Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updateat { get; set; }

    public string Createdby { get; set; } = null!;

    public string? Updatedby { get; set; }

    public virtual Permission? Permission { get; set; }

    public virtual Role? Role { get; set; }
}
