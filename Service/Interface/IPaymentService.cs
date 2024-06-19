using Data.Models;
using Data.ViewModel;
using Data.ViewModel.Payment;

namespace Service.Interface
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDetail>> GetAll();
        Task<ApiResult<PaymentDetailViewModel>> GetPaymentDetail(string username, int OrderId);
        Task<ApiResult<string>> MakePayment(PaymentInfoRequest paymentInfoRequest, string username);
        Task<ApiResult<PaymentDetailViewModel>> ShipCOD(string username, int OrderID);
        Task<ApiResult<bool>> UpdatePaymentStatus(int paymentId, int paymentStatus, int StatusOrderId);
        Task<ApiResult<bool>> ValidPayment(IQueryCollection querylists);
    }
}
