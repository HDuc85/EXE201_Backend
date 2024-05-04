using System;
using System.Collections.Generic;

namespace Exe201_backend.Models;

public partial class StoreItem
{
    public int Id { get; set; }

    public int? StoreId { get; set; }

    public int? ItemId { get; set; }

    public bool? IsActive { get; set; }

    public virtual Item? Item { get; set; }

    public virtual Store? Store { get; set; }
}
