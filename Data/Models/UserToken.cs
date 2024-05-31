using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Usertoken 
{
    public Guid UserId { get; set; }
    public string LoginProvider { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public string RefreshToken { get; set; }
    public virtual User User { get; set; } = null!;
}
