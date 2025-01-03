﻿using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Product
{
    public int Id { get; set; }

    public string? ProductName { get; set; }

    public int? QuantitySold { get; set; }

    public double? Rate { get; set; }

    public string? Description { get; set; }

    public Guid? Auther { get; set; }

    public virtual User? AutherNavigation { get; set; }

    public virtual ICollection<StoreItem> StoreItems { get; set; } = new List<StoreItem>();
    public virtual ICollection<ProductMedia> ProductMedia { get; set; } = new List<ProductMedia>();   

    public virtual ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();

    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}
