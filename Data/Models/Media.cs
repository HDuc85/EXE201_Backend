using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Media
{
    public int Id { get; set; }

    public string? MediaUrl { get; set; }

    public int? MediaTypeId { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<FeedbackMedia> FeedbackMedia { get; set; } = new List<FeedbackMedia>();

    public virtual ICollection<BoxMedia> BoxMedia { get; set; } = new List<BoxMedia>();
    public virtual ICollection<ProductMedia> ProductMedia { get; set; } = new List<ProductMedia>();


    public virtual MediaType? MediaType { get; set; }
}
