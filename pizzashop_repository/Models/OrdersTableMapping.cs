using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class OrdersTableMapping
{
    public int Id { get; set; }

    public int Tableid { get; set; }

    public int Orderid { get; set; }

    public string Name { get; set; } = null!;

    public int NoOfPerson { get; set; }

    public bool Isdeleted { get; set; }

    public DateTime Createdat { get; set; }

    public DateTime Updatedat { get; set; }

    public int? Createdby { get; set; }

    public int? Updatedby { get; set; }

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Table Table { get; set; } = null!;

    public virtual User? UpdatedbyNavigation { get; set; }
}
