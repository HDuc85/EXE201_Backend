using Data.Models;
using Data.ViewModel.Cart;
using Data.ViewModel.Order;

namespace Data.ViewModel.Store
{
    public class ItemStoreVm
    {
        public int Id { get; set; }
        public List<PVariantViewModel> pVariantViewModels { get; set; }
        public List<BoxViewModel> boxViewModels { get; set; }
    }
    public class PVariantViewModel
    {
        public string? Name { get; set; }   
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? Brand { get; set; }
        public string? Thumbnail { get; set; }
        public double? Price { get; set; }
        public double? Discount { get; set; }
        public int? Weight { get; set; }
    }
    public class BoxViewModel
    {
        public string? Name { get; set; }
        public string? Thumbnail { get; set; }
        public double? Price { get; set; }
        public double? Discount { get; set; }
    }

}
