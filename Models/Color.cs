using System;
using System.Collections.Generic;

namespace Exe201_backend.Models;

public partial class Color
{
    public int Id { get; set; }

    public string? ColorValue { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}
