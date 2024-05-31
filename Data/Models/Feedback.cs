using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Feedback
{
    public int Id { get; set; }

    public Guid? UserId { get; set; }

    public int? ItemId { get; set; }

    public int? OrderId { get; set; }

    public int? Rate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<FeedbackMedia> FeedbackMedia { get; set; } = new List<FeedbackMedia>();

    public virtual Item? Item { get; set; }

    public virtual Order? Order { get; set; }

    public virtual User? User { get; set; }
}
