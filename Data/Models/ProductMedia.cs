﻿namespace Data.Models
{
    public class ProductMedia
    {
        public int Id { get; set; }

        public int? ProductId { get; set; }

        public int? MediaId { get; set; }

        public bool? IsActive { get; set; }

        public virtual Product? Product { get; set; }

        public virtual Media? Media { get; set; }
    }
}
