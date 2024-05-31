using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class TagValue
{
    public int Id { get; set; }

    public int? TagId { get; set; }

    public string? Value { get; set; }

    public virtual ICollection<BoxTag> BoxTags { get; set; } = new List<BoxTag>();

    public virtual ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();

    public virtual Tag? Tag { get; set; }
}
