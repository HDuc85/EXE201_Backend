using System;
using System.Collections.Generic;

namespace Exe201_backend.Models;

public partial class Item
{
    public int Id { get; set; }

    public int? ItemTypeId { get; set; }

    public int? ItemId { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual Product? Item1 { get; set; }

    public virtual ICollection<ItemMedia> ItemMedia { get; set; } = new List<ItemMedia>();

    public virtual Box? ItemNavigation { get; set; }

    public virtual ItemType? ItemType { get; set; }

    public virtual ICollection<StoreItem> StoreItems { get; set; } = new List<StoreItem>();
}
