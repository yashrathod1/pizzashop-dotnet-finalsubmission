using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class Customer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public int NoOfPerson { get; set; }

    public DateTime Createdat { get; set; }

    public DateTime Updatedat { get; set; }

    public int? Createdby { get; set; }

    public int? Updatedby { get; set; }

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual ICollection<Order> Orders { get; } = new List<Order>();

    public virtual User? UpdatedbyNavigation { get; set; }

    public virtual ICollection<WaitingToken> WaitingTokens { get; } = new List<WaitingToken>();
}
