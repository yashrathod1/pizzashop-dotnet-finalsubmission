using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class Taxesandfee
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public decimal Value { get; set; }

    public bool Isenabled { get; set; }

    public bool Isdefault { get; set; }

    public bool Isdeleted { get; set; }

    public DateTime Createdat { get; set; }

    public DateTime Updatedat { get; set; }

    public int? Createdby { get; set; }

    public int? Updatedby { get; set; }

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual ICollection<OrderTaxesMapping> OrderTaxesMappings { get; } = new List<OrderTaxesMapping>();

    public virtual User? UpdatedbyNavigation { get; set; }
}
