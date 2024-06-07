﻿namespace Service.ViewModel.System
{

    public class CreateProductDTO
    {
        public string? ProductName { get; set; }
        public int? QuantitySold { get; set; }
        //public double? Rate { get; set; }
        public string? Description { get; set; }
        //public Guid? Auther { get; set; }
        public List<CreateProductVariantDTO> ProductVariants { get; set; }
    }
    public class CreateProductVariantDTO
    {
        public string? SizeName { get; set; }
        public string? BrandName { get; set; }
        public string? ColorName { get; set; }
        public IFormFile? Thumbnail { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }


}