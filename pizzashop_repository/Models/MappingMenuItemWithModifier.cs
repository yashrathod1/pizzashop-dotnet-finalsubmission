using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class MappingMenuItemWithModifier
{
    public int Id { get; set; }

    public int MenuItemId { get; set; }

    public int ModifierGroupId { get; set; }

    public int MinModifierCount { get; set; }

    public int MaxModifierCount { get; set; }

    public bool Isdeleted { get; set; }

    public DateTime Createdat { get; set; }

    public DateTime Updatedat { get; set; }

    public int? Createdby { get; set; }

    public int? Updatedby { get; set; }

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual MenuItem MenuItem { get; set; } = null!;

    public virtual Modifiergroup ModifierGroup { get; set; } = null!;

    public virtual User? UpdatedbyNavigation { get; set; }
}
