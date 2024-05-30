using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class StoreMember
{
    public int Id { get; set; }

    public Guid? MemberId { get; set; }

    public bool? IsActive { get; set; }

    public int? StoreId { get; set; }

    public virtual User? Member { get; set; }

    public virtual Store? Store { get; set; }
}
