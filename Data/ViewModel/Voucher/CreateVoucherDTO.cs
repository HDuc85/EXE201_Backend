namespace Data.ViewModel.Voucher
{

    public class CreateVoucherDTO
    { 
        public string? VoucherName { get; set; }

        public int? Type { get; set; }
        public string? Value { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
    }
 


}
