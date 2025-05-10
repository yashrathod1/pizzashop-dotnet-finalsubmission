using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class OrderItemsMapping
{
    public int Id { get; set; }

    public int Orderid { get; set; }

    public int Menuitemid { get; set; }

    public string ItemName { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal TotalPrice { get; set; }

    public string? Instruction { get; set; }

    public int? Preparedquantity { get; set; }

    public bool Isdeleted { get; set; }

    public virtual MenuItem Menuitem { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual ICollection<OrderItemModifier> OrderItemModifiers { get; } = new List<OrderItemModifier>();
}
