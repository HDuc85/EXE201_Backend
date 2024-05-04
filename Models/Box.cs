using System;
using System.Collections.Generic;

namespace Exe201_backend.Models;

public partial class Box
{
    public int Id { get; set; }

    public string? BoxName { get; set; }

    public int? QuantitySold { get; set; }

    public double? Rate { get; set; }

    public string? Description { get; set; }

    public double? Price { get; set; }

    public Guid? Auther { get; set; }

    public virtual User? AutherNavigation { get; set; }

    public virtual ICollection<BoxItem> BoxItems { get; set; } = new List<BoxItem>();

    public virtual ICollection<BoxTag> BoxTags { get; set; } = new List<BoxTag>();

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
