using System;
using System.Collections.Generic;

namespace Exe201_backend.Models;

public partial class FeedbackMedia
{
    public int Id { get; set; }

    public int? FeedbackId { get; set; }

    public int? MediaId { get; set; }

    public bool? IsActive { get; set; }

    public virtual Feedback? Feedback { get; set; }

    public virtual Media? Media { get; set; }
}
