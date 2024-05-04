using System;
using System.Collections.Generic;

namespace Exe201_backend.Models;

public partial class MediaType
{
    public int Id { get; set; }

    public string? MediaName { get; set; }

    public bool? IsActive { get; set; }
}
