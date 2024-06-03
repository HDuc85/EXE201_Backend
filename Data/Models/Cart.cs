using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Cart
{
    public int Id { get; set; }

    public Guid? UserId { get; set; }

    public int? ProductVariantId { get; set; }
    public int? BoxId { get; set; }

    public int? Quantity { get; set; }

    public virtual ProductVariant? ProductVariant { get; set; }
    public virtual Box? Box { get; set; }
}
