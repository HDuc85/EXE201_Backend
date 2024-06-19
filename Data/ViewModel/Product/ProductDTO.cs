namespace Data.ViewModel.Product
{

    public class ProductDTO
    {
        public string? ProductName { get; set; }
        public int? QuantitySold { get; set; }
        //public double? Rate { get; set; }
        public string? Description { get; set; }
        public Guid? Auther { get; set; }
        public List<ProductVariantDTO> ProductVariantDTO { get; set; }
    }
    public class ProductVariantDTO
    {
        public string? SizeName { get; set; }
        public string? BrandName { get; set; }
        public string? ColorName { get; set; }
        public string? Thumbnail { get; set; }
        public double? Price { get; set; }
        public int Quantity { get; set; }
    }

}
