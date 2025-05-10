using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int Orderid { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public bool PaymentStatus { get; set; }

    public DateTime Createdat { get; set; }

    public DateTime Updatedat { get; set; }

    public int? Createdby { get; set; }

    public int? Updatedby { get; set; }

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; } = new List<Order>();

    public virtual User? UpdatedbyNavigation { get; set; }
}
