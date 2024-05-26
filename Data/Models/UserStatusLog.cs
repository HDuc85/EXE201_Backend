using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class UserStatusLog
{
    public int Id { get; set; }

    public Guid? UserId { get; set; }

    public int? StatusId { get; set; }

    public string? TextLog { get; set; }

    public DateTime? LogAt { get; set; }

    public virtual Status? Status { get; set; }

    public virtual User? User { get; set; }
}
