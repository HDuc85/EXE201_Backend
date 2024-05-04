using System;
using System.Collections.Generic;

namespace Exe201_backend.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public DateOnly? Birthday { get; set; }

    public virtual ICollection<Box> Boxes { get; set; } = new List<Box>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<UserStatusLog> UserStatusLogs { get; set; } = new List<UserStatusLog>();
    public virtual ICollection<StoreMember> StoreMembers { get; set; } = new List<StoreMember>();


    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
