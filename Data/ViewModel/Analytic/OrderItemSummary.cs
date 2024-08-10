namespace Data.ViewModel.Analytic
{
    public class OrderItemSummary
    {
        public int? ProductVariantId { get; set; }
        public int? BoxId { get; set; }
        public int TotalQuantity { get; set; }
        public double TotalPrice { get; set; }
    }
}
