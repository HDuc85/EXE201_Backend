using System;
using System.Collections.Generic;

namespace Service.Models;

public partial class BoxTag
{
    public int Id { get; set; }

    public int? BoxId { get; set; }

    public int? TagVauleId { get; set; }

    public bool? IsActive { get; set; }

    public virtual Box? Box { get; set; }

    public virtual TagValue? TagVaule { get; set; }
}
