using Data.Models;

namespace Data.ViewModel.Cart
{
    public class CreateCartRequest
    {
       
        public List<BoxItemRequest>? boxs { get; set; }
        public List<ProductVariantItemRequest>? productVariants { get; set; }
    }
}
