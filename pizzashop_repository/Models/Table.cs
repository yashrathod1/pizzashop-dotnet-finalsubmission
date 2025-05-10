using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class Table
{
    public int Id { get; set; }

    public int Sectionid { get; set; }

    public string Name { get; set; } = null!;

    public int Capacity { get; set; }

    public bool Isdeleted { get; set; }

    public DateTime Createdat { get; set; }

    public DateTime Updatedat { get; set; }

    public int? Createdby { get; set; }

    public int? Updatedby { get; set; }

    public string Status { get; set; } = null!;

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual ICollection<OrdersTableMapping> OrdersTableMappings { get; } = new List<OrdersTableMapping>();

    public virtual Section Section { get; set; } = null!;

    public virtual User? UpdatedbyNavigation { get; set; }

    public virtual ICollection<WaitingToken> WaitingTokens { get; } = new List<WaitingToken>();
}
