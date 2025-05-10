using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class Feedback
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int? Food { get; set; }

    public int? Service { get; set; }

    public int? Ambience { get; set; }

    public int? Avgrating { get; set; }

    public string? Comments { get; set; }

    public DateTime Createdat { get; set; }

    public DateTime Updatedat { get; set; }

    public int? Createdby { get; set; }

    public int? Updatedby { get; set; }

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual User? UpdatedbyNavigation { get; set; }
}
