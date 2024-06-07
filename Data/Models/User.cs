using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class User : IdentityUser<Guid>
{
    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string? Address { get; set; }

    public DateOnly? Birthday { get; set; }

    public virtual ICollection<Box> Boxes { get; set; } = new List<Box>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<StoreMember> StoreMembers { get; set; } = new List<StoreMember>();

    public virtual ICollection<UserStatusLog> UserStatusLogs { get; set; } = new List<UserStatusLog>();

    public virtual ICollection<IdentityUserClaim<Guid>> Userclaims { get; set; } = new List<IdentityUserClaim<Guid>>();

    public virtual ICollection<IdentityUserRole<Guid>> UserRoles { get; set; } = new List<IdentityUserRole<Guid>>();
    public virtual ICollection<IdentityUserLogin<Guid>> Userlogins { get; set; } = new List<IdentityUserLogin<Guid>>();

    public virtual ICollection<IdentityUserToken<Guid>> Usertokens { get; set; } = new List<IdentityUserToken<Guid>>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
