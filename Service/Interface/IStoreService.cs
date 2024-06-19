using Data.Models;
using Data.ViewModel;
using Data.ViewModel.Store;

namespace Service.Interface
{
    public interface IStoreService
    {
        Task<StoreVM> CreateStore(string username,StoreCreateRequest request);
        Task<ApiResult> DeleteStore(string username, int storeId);
        Task<ApiResult> DeleteStoreItem(string username, ItemRequest items);
        Task<ApiResult> DeleteStoreMember(string username, MemberRequest request);
        Task<IEnumerable<StoreVM>> GetAll();
        Task<ItemStoreVm> GetAllItemStore(string username, int storeId);
        Task<MemberStoreVM> GetAllMember(string username, int storeId);
        Task<StoreVM> GetById(int id);
        Task<ApiResult> InsertItem(string username, ItemRequest request);
        Task<ApiResult> InsetMember(string username, MemberRequest request);
        Task<ApiResult> UpdateStore(string username, UpdateStoreRequest request);
    }
}
