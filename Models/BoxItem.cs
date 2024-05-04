using System;
using System.Collections.Generic;

namespace Exe201_backend.Models;

public partial class BoxItem
{
    public int Id { get; set; }

    public int? BoxId { get; set; }

    public int? ProductVariantId { get; set; }

    public int? Quantity { get; set; }

    public virtual Box? Box { get; set; }

    public virtual ProductVariant? ProductVariant { get; set; }
}
