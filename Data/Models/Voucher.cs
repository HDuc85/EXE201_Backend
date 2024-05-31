using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Voucher
{
    public int Id { get; set; }

    public string? VoucherName { get; set; }

    public int? Type {  get; set; }
    public string? Value { get; set; }
    public DateTime? DateStart { get; set; }
    public DateTime? DateEnd { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
