using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class PaymentStatus
{
    public int Id { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<PaymentDetail> PaymentDetails { get; set; } = new List<PaymentDetail>();
}
