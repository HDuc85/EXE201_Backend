using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Cart
{
    public int Id { get; set; }

    public Guid? UserId { get; set; }

    public int? ItemId { get; set; }

    public int? Quantity { get; set; }

    public virtual Item? Item { get; set; }

    public virtual User? User { get; set; }
}
