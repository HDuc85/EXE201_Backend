using Data.Models;
using Data.ViewModel;
using Data.ViewModel.Cart;

namespace Service.Interface
{
    public interface ICartService
    {
        Task<ApiResult<bool>> AddCart(string username,CreateCartRequest createCartRequest);
        Task<(string, IEnumerable<Cart>)> CheckQuantity(IEnumerable<Cart> carts);
        Task<ApiResult<int>> DeleteAllCart(string Username);
        Task<ApiResult<int>> DeleteCart(string username ,DeleteCartRequest request);
        Task<ApiResult<List<CartViewModel>>> GetCart(string username);
        Task<ApiResult<List<CartViewModel>>> GetCartBySize(CartBySizeRequest cartBySizeRequest);
        Task<ApiResult<bool>> SingleAdd(SingleAddRequest singleAddRequest);
        Task<ApiResult<bool>> UpdateCart(UpdateCartRequest updateCartRequest);
    }
}
