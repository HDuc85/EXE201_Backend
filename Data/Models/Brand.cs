using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Brand
{
    public int Id { get; set; }

    public string? BrandValue { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}
