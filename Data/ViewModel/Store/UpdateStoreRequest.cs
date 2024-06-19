namespace Data.ViewModel.Store
{
    public class UpdateStoreRequest
    {
        public int StoreId { get; set; }
        public string? StoreName { get; set; }
        public IFormFile? avatar { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Status { get; set; }
    }
}
