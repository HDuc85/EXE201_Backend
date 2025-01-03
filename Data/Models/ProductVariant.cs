﻿using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class ProductVariant
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public int? SizeId { get; set; }

    public int? ColorId { get; set; }

    public int? BrandId { get; set; }

    public string? Thumbnail { get; set; }

    public double? Price { get; set; }
    public int? weight { get; set; }

    public int Quantity { get; set; }
    public double? Discount { get; set; }
    public bool? IsActive { get; set; }

    public virtual ICollection<BoxItem> BoxItems { get; set; } = new List<BoxItem>();
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
    public virtual ICollection<OrderItem> OrderItem { get; set; } = new List<OrderItem>();
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();


    public virtual Brand? Brand { get; set; }

    public virtual Color? Color { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Size? Size { get; set; }
}
