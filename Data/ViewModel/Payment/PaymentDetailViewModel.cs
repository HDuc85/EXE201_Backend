namespace Data.ViewModel.Payment
{
    public class PaymentDetailViewModel
    {
        public int OrderId { get; set; }
        public long? Amount { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? Desc { get; set; }
        public DateTime? PayDate { get; set; } 
        public string? PaymentStatus { get; set; } 

    }
}
