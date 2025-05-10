using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class OrderTaxesMapping
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int TaxId { get; set; }

    public string TaxName { get; set; } = null!;

    public decimal TaxValue { get; set; }

    public decimal? TaxAmount { get; set; }

    public DateTime Createdat { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Taxesandfee Tax { get; set; } = null!;
}
