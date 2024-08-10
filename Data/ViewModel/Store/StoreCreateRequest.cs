namespace Data.ViewModel.Store
{
    public class StoreCreateRequest
    {
        public string StoreName { get; set; }
        public IFormFile? Avatar {  get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }

    }
}
