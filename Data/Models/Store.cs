using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Store
{
    public int Id { get; set; }

    public string? StoreName { get; set; }

    public string? Address { get; set; }
    public string? Phone { get; set; }

    public string? Avatar { get; set; }

    public double? Rate { get; set; }

    public string? Description { get; set; }

    public int? ProductQuantity { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<StoreItem> StoreItems { get; set; } = new List<StoreItem>();

    public virtual ICollection<StoreMember> StoreMembers { get; set; } = new List<StoreMember>();
}
