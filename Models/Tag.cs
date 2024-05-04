using System;
using System.Collections.Generic;

namespace Exe201_backend.Models;

public partial class Tag
{
    public int Id { get; set; }

    public string? TagName { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<TagValue> TagValues { get; set; } = new List<TagValue>();
}
