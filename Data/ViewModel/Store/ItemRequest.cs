using Data.Models;

namespace Data.ViewModel.Store
{
    public class ItemRequest
    {
        public int StoreId { get; set; }
        public List<int>? productVId { get; set; }
        public List<int>? BoxsId { get; set; }
    } 
}
