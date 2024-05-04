using System;
using System.Collections.Generic;

namespace Exe201_backend.Models;

public partial class StoreMember
{
    public int Id { get; set; }

    public Guid? MemberId { get; set; }

  

    public int? StoreId { get; set; }

    public bool? IsActive { get; set; }


    public virtual Store? Store { get; set; }

    public virtual User? User { get; set; }
}
