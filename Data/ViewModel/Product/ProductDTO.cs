using Data.Models;

namespace Data.ViewModel.Product
{

    public class ProductDTO
    {
        public string? ProductName { get; set; }
        public int? QuantitySold { get; set; }
        //public double? Rate { get; set; }
        public string? Description { get; set; }
        public Guid? Auther { get; set; }

        public virtual ICollection<ProductMediaDTO> ProductMedia { get; set; } = new List<ProductMediaDTO>();

        public virtual ICollection<ProductTagDTO> ProductTags { get; set; } = new List<ProductTagDTO>();

        public virtual ICollection<ProductVariantDTO> ProductVariants { get; set; } = new List<ProductVariantDTO>();
    }

}
