using System;
using System.Collections.Generic;

namespace Service.Models;

public partial class ItemMedia
{
    public int Id { get; set; }

    public int? ItemId { get; set; }

    public int? MediaId { get; set; }

    public bool? IsActive { get; set; }

    public virtual Item? Item { get; set; }

    public virtual Media? Media { get; set; }
}
