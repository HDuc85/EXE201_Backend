using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class OrderStatus
{
    public int Id { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<OrderStatusLog> OrderStatusLogs { get; set; } = new List<OrderStatusLog>();
}
