using System.Drawing;

namespace Data.ViewModel.Cart
{
    public class CartViewModel
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public ProductVariantViewModel productVariantViewModel { get; set; }
        public int? Quantity { get; set; }
        public double? Price { get; set; }
        public string? thumbnail { get; set; }
        public double? discount { get; set; }
    }
    public class ProductVariantViewModel
    {
        public string? size { get; set; }
        public string? color { get; set; }
        public string? brand { get; set; }
    }
}
