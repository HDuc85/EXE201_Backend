namespace Data.ViewModel.Box
{
    public class CreateBoxRequest
    {
        public string? BoxName { get; set; }
        public double? Price { get; set; }
        public string? Thumbnail { get; set; }
        public double? Discount { get; set; }
        public string? Description { get; set; }
    }
}
