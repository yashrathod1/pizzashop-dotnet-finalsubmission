using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class Country
{
    public int Id { get; set; }

    public string? Countryname { get; set; }

    public virtual ICollection<State> States { get; } = new List<State>();

    public virtual ICollection<User> Users { get; } = new List<User>();
}
