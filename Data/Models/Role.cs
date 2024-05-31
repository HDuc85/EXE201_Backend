using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Role : IdentityRole<Guid>
{
   

    public virtual ICollection<Roleclaim> Roleclaims { get; set; } = new List<Roleclaim>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
