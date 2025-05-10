using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class MenuItem
{
    public int Id { get; set; }

    public int? Categoryid { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public decimal Rate { get; set; }

    public int Quantity { get; set; }

    public string UnitType { get; set; } = null!;

    public bool IsAvailable { get; set; }

    public bool IsdefaultTax { get; set; }

    public decimal TaxPercentage { get; set; }

    public string? ShortCode { get; set; }

    public string? Description { get; set; }

    public string? ItemImage { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public bool Isfavourite { get; set; }

    public virtual Category? Category { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<MappingMenuItemWithModifier> MappingMenuItemWithModifiers { get; } = new List<MappingMenuItemWithModifier>();

    public virtual ICollection<OrderItemsMapping> OrderItemsMappings { get; } = new List<OrderItemsMapping>();

    public virtual User? UpdatedByNavigation { get; set; }
}
