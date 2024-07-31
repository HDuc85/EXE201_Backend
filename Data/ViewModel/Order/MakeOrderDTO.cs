namespace Data.ViewModel.Order
{
    public class MakeOrderDTO : InforAddressDTO
    {
        //public double totalPrice {  get; set; }
        public string OrderService {  get; set; }
        public string? OrderNote { get; set; }
        //public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        //public string SenderPhone { get; set; }
        public string ReceiverPhone { get; set; }
        public string UserIP {  get; set; }
        public int PaymentType {  get; set; }
        //public int Weight { get; set; }
    }
}
