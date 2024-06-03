﻿using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class OrderItem
{
    public int? OrderId { get; set; }

    public int? ItemId { get; set; }

    public int? Quantity { get; set; }
    
    public double? Price { get; set; }
    public virtual Item? Item { get; set; }

    public virtual Order? Order { get; set; }
}
