using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Item
{
    public int Id { get; set; }

    public int? ItemTypeId { get; set; }

    public bool? IsActive { get; set; }

    public int? ProductId { get; set; }

    public int? BoxId { get; set; }

    public virtual Box? Box { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<ItemMedia> ItemMedia { get; set; } = new List<ItemMedia>();

    public virtual ItemType? ItemType { get; set; }

    public virtual Product? Product { get; set; }

    public virtual ICollection<StoreItem> StoreItems { get; set; } = new List<StoreItem>();
}
