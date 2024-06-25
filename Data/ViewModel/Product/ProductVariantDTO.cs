using Data.Models;

namespace Data.ViewModel.Product
{ 
    public class ProductVariantDTO
    {
        public int? SizeId { get; set; }
        public int? BrandId { get; set; }
        public int? ColorId { get; set; }
        public string? Thumbnail { get; set; }
        public double? Price { get; set; }
        public int Quantity { get; set; }
        public bool? IsActive { get; set; }
        public virtual Brand? Brand { get; set; }

        public virtual Color? Color { get; set; }

        public virtual Data.Models.Product? Product { get; set; }

        public virtual Size? Size { get; set; }
    }

}
