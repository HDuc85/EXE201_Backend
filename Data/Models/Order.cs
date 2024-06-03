using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Order
{
    public int Id { get; set; }

    public Guid? UserId { get; set; }

    public double? Price { get; set; }

    public int? PaymentId { get; set; }

    public int? VoucherId { get; set; }
    public double? ShipPrice { get; set; }
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<OrderStatusLog> OrderStatusLogs { get; set; } = new List<OrderStatusLog>();
    public virtual Voucher? Voucher { get; set; }
    public virtual PaymentDetail? Payment { get; set; }

    public virtual User? User { get; set; }
}
