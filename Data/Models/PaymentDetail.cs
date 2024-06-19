using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class PaymentDetail
{
    public int Id { get; set; }


    public long? Amount { get; set; }

    public string? Description { get; set; }


    public string? BankCode { get; set; }
    public string? BankTranNo {  get; set; }
    public DateTime? PayDate { get; set; } 

    public long? TransactionNo { get; set; }

    public int? PaymentStatusId { get; set; }
    public DateTime? CreatedDate { get; set; }
    

    public virtual PaymentStatus? Status { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
