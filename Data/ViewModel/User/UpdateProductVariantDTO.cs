﻿namespace Service.ViewModel.System
{

    public class UpdateProductVariantDTO
    {
        public string? SizeName { get; set; }
        public string? BrandName { get; set; }
        public string? ColorName { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}