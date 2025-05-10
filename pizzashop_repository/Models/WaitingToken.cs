using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class WaitingToken
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int NoOfPersons { get; set; }

    public int SectionId { get; set; }

    public int? TableId { get; set; }

    public bool IsAssign { get; set; }

    public bool Isdeleted { get; set; }

    public DateTime Createdat { get; set; }

    public DateTime Updatedat { get; set; }

    public int? Createdby { get; set; }

    public int? Updatedby { get; set; }

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Section Section { get; set; } = null!;

    public virtual Table? Table { get; set; }

    public virtual User? UpdatedbyNavigation { get; set; }
}
