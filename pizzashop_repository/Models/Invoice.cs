using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class Invoice
{
    public int Id { get; set; }

    public int Orderid { get; set; }

    public string InvoiceNumber { get; set; } = null!;

    public DateTime Createdat { get; set; }

    public int? Createdby { get; set; }

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual Order Order { get; set; } = null!;
}
