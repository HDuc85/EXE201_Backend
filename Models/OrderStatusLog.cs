using System;
using System.Collections.Generic;

namespace Exe201_backend.Models;

public partial class OrderStatusLog
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public int? StatusId { get; set; }

    public string? TextLog { get; set; }

    public DateTime? LogAt { get; set; }

    public virtual Order? Order { get; set; }

    public virtual OrderStatus? Status { get; set; }
}
