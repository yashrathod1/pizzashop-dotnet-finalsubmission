using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class City
{
    public int Id { get; set; }

    public string? Cityname { get; set; }

    public int? Stateid { get; set; }

    public virtual State? State { get; set; }

    public virtual ICollection<User> Users { get; } = new List<User>();
}
