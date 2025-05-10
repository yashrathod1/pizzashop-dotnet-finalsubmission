using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class OrderItemModifier
{
    public int Id { get; set; }

    public int Orderitemid { get; set; }

    public int Modifierid { get; set; }

    public string ModifierName { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual Modifier Modifier { get; set; } = null!;

    public virtual OrderItemsMapping Orderitem { get; set; } = null!;
}
