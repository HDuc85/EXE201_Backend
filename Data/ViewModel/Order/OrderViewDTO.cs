using Data.Models;

namespace Data.ViewModel.Order
{
    public class OrderViewDTO
    {
        public int? OrderId { get; set; }
        public string? PaymentStatus { get; set; }
        public double? ShipPrice { get; set; }

        public double? OrderPrice {  get; set; }
        public string? TrackingNumber { get; set; }
        public string? OrderStatus { get; set; }

    }
}
