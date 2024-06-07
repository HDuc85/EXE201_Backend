using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Data.ViewModel.Cart
{
    
    public class UpdateCartRequest
    {
        public string Username {  get; set; }
        public List<CartItem>? Items { get; set;}
    }

    public class CartItem
    {
        public int Type { get; set; }
        public int Id { get; set; }
        
        public int Quantity { get; set; }
    }
    
}
