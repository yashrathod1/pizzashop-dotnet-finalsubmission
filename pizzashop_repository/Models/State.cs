using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class State
{
    public int Id { get; set; }

    public string? Statename { get; set; }

    public int? Countryid { get; set; }

    public virtual ICollection<City> Cities { get; } = new List<City>();

    public virtual Country? Country { get; set; }

    public virtual ICollection<User> Users { get; } = new List<User>();
}
