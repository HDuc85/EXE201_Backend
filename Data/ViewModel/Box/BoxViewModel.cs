namespace Data.ViewModel.Box
{
    public class BoxViewModel
    {
        public int Id { get; set; }
        public string? BoxName { get; set; }
        public int? QuantitySold { get; set; }
        public double? Rate { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public string? Thumbnail { get; set; }
        public double? Discount { get; set; }
        public Guid? Auther { get; set; }
    }
}
