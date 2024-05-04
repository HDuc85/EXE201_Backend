using System;
using System.Collections.Generic;

namespace Exe201_backend.Models;

public partial class Status
{
    public int Id { get; set; }

    public string? Status1 { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<UserStatusLog> UserStatusLogs { get; set; } = new List<UserStatusLog>();
}
