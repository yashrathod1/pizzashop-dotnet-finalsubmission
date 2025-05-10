using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class Modifiergroup
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool Isdeleted { get; set; }

    public DateTime Createdat { get; set; }

    public DateTime Updatedat { get; set; }

    public int? Createdby { get; set; }

    public int? Updatedby { get; set; }

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual ICollection<MappingMenuItemWithModifier> MappingMenuItemWithModifiers { get; } = new List<MappingMenuItemWithModifier>();

    public virtual ICollection<Modifiergroupmodifier> Modifiergroupmodifiers { get; } = new List<Modifiergroupmodifier>();

    public virtual User? UpdatedbyNavigation { get; set; }
}
