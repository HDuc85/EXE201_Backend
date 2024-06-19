using Data.Models;
using Data.ViewModel;
using Data.ViewModel.Order;

namespace Service.Interface
{
    public interface IOrderService
    {
        Task<(Object, int)> CheckShipPrice(InforAddressDTO inforAddress);
        Task<ApiResult<IEnumerable<OrderViewDTO>>> GetAll(string username);
        Task<ApiResult<OrderViewDTO>> GetById(string username, int OrderId);
        Task<ApiResult<MakeOrderReponseDTO>> MakeOrder(string username, MakeOrderDTO makeOrder);
        Task<ApiResult<bool>> RemoveOrder(string username, int? orderId);
        Task<ApiResult<TotalPriceDTO>> TotalPriceItem(string username, TotalPriceItemDTO makeOrder);
        Task<ApiResult<bool>> UpdateOrderStatus(string username, int OrderId, int OrderStatusId);
    }
}
