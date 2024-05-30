using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Roleclaim 
{
    public int Id { get; set; }

    public Guid RoleId { get; set; }

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }

    public virtual Role Role { get; set; } = null!;
}
