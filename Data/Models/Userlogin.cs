using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Userlogin 
{
    public string LoginProvider { get; set; } = null!;

    public string ProviderKey { get; set; } = null!;

    public string ProviderDisplayName { get; set; } = null!;
    public Guid UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
