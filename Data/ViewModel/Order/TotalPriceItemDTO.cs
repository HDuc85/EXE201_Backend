using Data.ViewModel.Cart;
using Microsoft.VisualBasic;

namespace Data.ViewModel.Order
{
    public class TotalPriceItemDTO
    {
        public int? voucherId {  get; set; }
        public List<CartItem> ListCartItem { get; set; }
    }
}
