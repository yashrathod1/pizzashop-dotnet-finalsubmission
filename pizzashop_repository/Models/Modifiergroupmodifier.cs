using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class Modifiergroupmodifier
{
    public int Id { get; set; }

    public int Modifierid { get; set; }

    public int Modifiergroupid { get; set; }

    public bool Isdeleted { get; set; }

    public virtual Modifier Modifier { get; set; } = null!;

    public virtual Modifiergroup Modifiergroup { get; set; } = null!;
}
