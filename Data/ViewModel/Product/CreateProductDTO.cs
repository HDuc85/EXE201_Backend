namespace Data.ViewModel.Product
{

    public class CreateProductDTO
    {
        public string? ProductName { get; set; }
        public int? QuantitySold { get; set; }
        public double? Rate { get; set; }
        public string? Description { get; set; }
        public Guid? Auther { get; set; }
        public List<CreateProductVariantDTO> ProductVariants { get; set; }
    }
}
