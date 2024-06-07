using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class StoreItem
{
    public int Id { get; set; }

    public int? StoreId { get; set; }

    public int? ProductId { get; set; }
    public int? BoxId { get; set; }

    public bool? IsActive { get; set; }

    public virtual Product? Product{ get; set; }
    public virtual Box? Box { get; set; }

    public virtual Store? Store { get; set; }
}
