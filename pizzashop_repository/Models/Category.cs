using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updateat { get; set; }

    public string Createdby { get; set; } = null!;

    public string? Updatedby { get; set; }

    public virtual ICollection<MenuItem> MenuItems { get; } = new List<MenuItem>();
}
