using Data.ViewModel.Cart;

namespace Data.ViewModel.Order
{
    public class InforAddressDTO
    {
        public string SenderAddress { get; set; }
        public string ReceiverAddress { get; set; }
        public List<CartItem> ListCartItem { get; set; }
    }

  
}
