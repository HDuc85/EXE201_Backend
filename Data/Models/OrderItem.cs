using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class OrderItem
{
    public int Id { get; set; }
    public int? OrderId { get; set; }

    public int? ProductVariantId { get; set; }
    public int? BoxId { get; set; }

    public int? Quantity { get; set; }
    
    public double? Price { get; set; }
    public virtual ProductVariant? ProductVariant { get; set; }

    public virtual Box? Box { get; set; }
    public virtual Order? Order { get; set; }
}
