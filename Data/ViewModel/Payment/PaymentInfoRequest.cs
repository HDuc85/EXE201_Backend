using Microsoft.VisualBasic;

namespace Data.ViewModel.Payment
{
    public class PaymentInfoRequest
    {
        public int OrderId { get; set; }
       
        public string? ip {  get; set; }
    }
}
