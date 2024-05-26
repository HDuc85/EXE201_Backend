using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Media
{
    public int Id { get; set; }

    public string? MediaUrl { get; set; }

    public string? MediaTypeId { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<FeedbackMedia> FeedbackMedia { get; set; } = new List<FeedbackMedia>();

    public virtual ICollection<ItemMedia> ItemMedia { get; set; } = new List<ItemMedia>();
}
