namespace Data.ViewModel.Cart
{
    public class DeleteCartRequest
    {
        public string Username { get; set; }
        public List<int>? ListProductVariantId {  get; set; }
        public List<int>? ListBoxId { get; set; }
    }
}
