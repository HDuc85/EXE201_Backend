using Data.Models;
using Data.ViewModel.Payment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Service.Helper.VNPay;
using Service.Interface;
using Service.Service;

namespace Exe201_backend.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        public PaymentController(IPaymentService paymentService, IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _paymentService = paymentService;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Payment( [FromQuery]PaymentInfoRequest paymentInfoRequest)
        {
            string vnp_Returnurl = _configuration["VNPay:vnp_Returnurl"];
            string vnp_Url = _configuration["VNPay:vnp_Url"];
            string vnp_TmnCode = _configuration["VNPay:vnp_TmnCode"];
            string vnp_HashSecret = _configuration["VNPay:vnp_HashSecret"];

            if (string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret))
            {
                return BadRequest("Server chưa cấu hình tham số cho VNPay ");
            }

            var orderdetail = await _unitOfWork.RepositoryOrder.GetById(paymentInfoRequest.OrderId);
            if(orderdetail == null)
            {
                return BadRequest("Invalid Order");
            }
            if(orderdetail.PaymentId == null)
            {
                var paymentDetail = new PaymentDetail
                {
                  
                    PaymentStatusId = 0,

                };
                await _unitOfWork.RepositoryPaymentDetail.Insert(paymentDetail);
                await _unitOfWork.RepositoryPaymentDetail.Commit();
                var order = _unitOfWork.RepositoryPaymentDetail.Table.OrderByDescending(x => x.Id).FirstOrDefault();

                orderdetail.PaymentId = order.Id;
                await _unitOfWork.CommitAsync();
            }

            var payment = await _unitOfWork.RepositoryPaymentDetail.GetById(orderdetail.PaymentId);

            payment.Amount = Convert.ToInt64(orderdetail.Price + orderdetail.ShipPrice);
            payment.CreatedDate = DateTime.Now;
            payment.Description = "payment order no." + orderdetail.Id;
            
            _unitOfWork.RepositoryPaymentDetail.Update(payment);


            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (payment.Amount * 100 ).ToString());

          
            vnpay.AddRequestData("vnp_CreateDate", payment.CreatedDate?.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", paymentInfoRequest.ip.ToString());

            
            vnpay.AddRequestData("vnp_Locale", "vn");
            
            vnpay.AddRequestData("vnp_OrderInfo", "payment order no." + paymentInfoRequest.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); 
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", paymentInfoRequest.OrderId.ToString());

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            var orderlog = _unitOfWork.RepositoryOrderStatusLog.Insert(new OrderStatusLog
            {
                LogAt = DateTime.Now,
                OrderId = paymentInfoRequest.OrderId,
                StatusId = 2,
                TextLog = $"Order No.{orderdetail.Id} is make payment with VNPAY at {DateTime.Now}"
            });
            await _unitOfWork.CommitAsync();

            return Ok(paymentUrl) ;
            
        }

        [HttpGet("/ReturnUrl")]
        public async Task<IActionResult> ReturnVNPayPayment()
        {
            
            var querylists = HttpContext.Request.Query;
            string vnp_HashSecret = _configuration["VNPay:vnp_HashSecret"];

            VnPayLibrary vnpay = new VnPayLibrary();
            var securehash =  querylists.FirstOrDefault(x => x.Key == "vnp_SecureHash");
            if(vnpay.ValidateSignature(securehash.Value, vnp_HashSecret))
            {
                return BadRequest("Invaild Payment request");
            }

            PaymentDetail paymentDetail = new PaymentDetail();
            string vnp_ResponseCode = string.Empty;
            foreach (var key in querylists)
            {
                switch (key.Key)
                {
                    case "vnp_Amount":
                        paymentDetail.Amount = Convert.ToInt64(key.Value) / 100;
                        break;
                    case "vnp_TransactionNo":
                        paymentDetail.TransactionNo = Convert.ToInt64(key.Value);
                        break;
                    case "vnp_TxnRef":
                        paymentDetail.Id = Convert.ToInt32(key.Value);
                        break;
                    case "vnp_ResponseCode":
                        vnp_ResponseCode = key.Value;
                        break;
                    case "vnp_BankCode":
                        paymentDetail.BankCode = key.Value;
                        break;
                    case "vnp_BankTranNo":
                        paymentDetail.BankTranNo = key.Value;
                        break;
                    case "vnp_PayDate":
                        paymentDetail.PayDate = DateTime.ParseExact(key.Value, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                        break;
                }
            }
            var order = await _unitOfWork.RepositoryOrder.GetById(paymentDetail.Id);
            if (order == null) {
                return BadRequest("Invalid Order");
            }
            var update = await _unitOfWork.RepositoryPaymentDetail.GetSingleByCondition(i => i.Id == order.PaymentId);
            if(vnp_ResponseCode == "00")
            {
                update.PaymentStatusId = 2;
                update.BankTranNo = paymentDetail.BankTranNo;
                update.PayDate = paymentDetail.PayDate;
                update.Amount = paymentDetail.Amount;
                update.TransactionNo = paymentDetail.TransactionNo;
                update.BankCode =   paymentDetail.BankCode;
                _unitOfWork.RepositoryPaymentDetail.Update(update);
                try
                {
                    var orderlog = _unitOfWork.RepositoryOrderStatusLog.Insert(new OrderStatusLog
                    {
                        LogAt = DateTime.Now,
                        OrderId = order.Id,
                        StatusId = 4,
                        TextLog = $"Order No.{order.Id} pay successfully at {DateTime.Now}"
                    });
                    await _unitOfWork.CommitAsync();
                    return Ok("Thanh toán thành công");

                }
                catch {
                    throw new Exception("Lỗi khi cập nhật database");
                }
            }
            else
            {
                update.PaymentStatusId = 1;
                try
                {
                    var orderlog = _unitOfWork.RepositoryOrderStatusLog.Insert(new OrderStatusLog
                    {
                        LogAt = DateTime.Now,
                        OrderId = order.Id,
                        StatusId = 3,
                        TextLog = $"Order No.{order.Id} pay fail at {DateTime.Now}"
                    });
                    await _unitOfWork.CommitAsync();
                return BadRequest("Thanh toán thất bại");
                  

                }
                catch
                {
                    throw new Exception("Lỗi khi cập nhật database");
                }

            }



        }

     

    }
}
