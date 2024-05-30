using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Userclaim 
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }

    public virtual User User { get; set; } = null!;
}
