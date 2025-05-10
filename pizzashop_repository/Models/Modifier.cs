using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class Modifier
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public string Unittype { get; set; } = null!;

    public int Quantity { get; set; }

    public string? Description { get; set; }

    public bool Isdeleted { get; set; }

    public DateTime Createdat { get; set; }

    public DateTime Updatedat { get; set; }

    public int? Createdby { get; set; }

    public int? Updatedby { get; set; }

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual ICollection<Modifiergroupmodifier> Modifiergroupmodifiers { get; } = new List<Modifiergroupmodifier>();

    public virtual ICollection<OrderItemModifier> OrderItemModifiers { get; } = new List<OrderItemModifier>();

    public virtual User? UpdatedbyNavigation { get; set; }
}
