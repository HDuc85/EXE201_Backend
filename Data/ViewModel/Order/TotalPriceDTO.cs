namespace Data.ViewModel.Order
{
    public class TotalPriceDTO
    {
        public double TotalPrice { get; set; }
       
        public List<ItemOrder> ItemOrders { get; set; }
    }
    public class ItemOrder
    {
        public int ItemType { get; set; }
        public string ItemName { get; set; }
        public List<Type>? ItemDescription { get; set; }
        public double Price { get; set; }
        public string? ItemUrl { get; set; }
        public int Quantity { get; set; }
    }
    public class Type
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
