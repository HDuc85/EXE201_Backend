using System;
using System.Collections.Generic;

namespace Exe201_backend.Models;

public partial class ProductTag
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public int? TagVauleId { get; set; }

    public bool? IsActive { get; set; }

    public virtual Product? Product { get; set; }

    public virtual TagValue? TagVaule { get; set; }
}
