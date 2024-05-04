using System;
using System.Collections.Generic;

namespace Exe201_backend.Models;

public partial class Role
{
    public int Id { get; set; }

    public string? Role1 { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
