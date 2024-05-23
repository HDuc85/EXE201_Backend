using System;
using System.Collections.Generic;

namespace Service.Models;

public partial class PaymentDetail
{
    public int Id { get; set; }

    public string? PaymentType { get; set; }

    public string? AccountNumber { get; set; }

    public double? Amount { get; set; }

    public string? Description { get; set; }

    public DateTime? TransactionDateTime { get; set; }

    public string? PaymentLinkId { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
