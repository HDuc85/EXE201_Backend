using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class ItemType
{
    public int Id { get; set; }

    public string? Type { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
