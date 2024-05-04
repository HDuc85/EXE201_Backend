﻿using System;
using System.Collections.Generic;

namespace Exe201_backend.Models;

public partial class Store
{
    public int Id { get; set; }

    public string? StoreName { get; set; }

    public string? Location { get; set; }

    public int? Avatar { get; set; }

    public double? Rate { get; set; }

    public string? Description { get; set; }

    public int? ProductQuantity { get; set; }

    public int? StatusId { get; set; }

    public virtual ICollection<StoreItem> StoreItems { get; set; } = new List<StoreItem>();

    public virtual ICollection<StoreMember> StoreMembers { get; set; } = new List<StoreMember>();
}
