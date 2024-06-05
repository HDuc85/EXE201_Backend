using Data.ViewModel;
using Data.ViewModel.Cart;

namespace Service.Interface
{
    public interface ICartService
    {
        Task<ApiResult<bool>> AddCart(CreateCartRequest createCartRequest);
        Task<ApiResult<int>> DeleteAllCart(string Username);
        Task<ApiResult<int>> DeleteCart(DeleteCartRequest request);
        Task<ApiResult<List<CartViewModel>>> GetCart(string username);
        Task<ApiResult<List<CartViewModel>>> GetCartBySize(CartBySizeRequest cartBySizeRequest);
        Task<ApiResult<bool>> SingleAdd(SingleAddRequest singleAddRequest);
        Task<ApiResult<bool>> UpdateCart(UpdateCartRequest updateCartRequest);
    }
}
