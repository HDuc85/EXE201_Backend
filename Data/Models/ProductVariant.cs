using System;
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

    public int? Quantity { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<BoxItem> BoxItems { get; set; } = new List<BoxItem>();

    public virtual Brand? Brand { get; set; }

    public virtual Color? Color { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Size? Size { get; set; }
}
