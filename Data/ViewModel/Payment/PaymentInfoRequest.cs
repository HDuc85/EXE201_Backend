using Microsoft.VisualBasic;

namespace Data.ViewModel.Payment
{
    public class PaymentInfoRequest
    {
        public int OrderId { get; set; }
        public string? locale { get; set; } = "VN";
        public string? ip {  get; set; }
    }
}
