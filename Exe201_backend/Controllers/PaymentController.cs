using Data.Models;
using Data.ViewModel.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Service.Helper.Header;
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
       
        private readonly IUnitOfWork _unitOfWork;
        public PaymentController(IPaymentService paymentService, IUnitOfWork unitOfWork)
        {
            _paymentService = paymentService;
           
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// Make payment, return vnpayUrl for payment (Input must have IP client)
        /// </summary>
        /// <param name="paymentInfoRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Payment( int OrderId, string clientIp)
        {
            var username = User.GetUserName();
            var result = await _paymentService.MakePayment(new PaymentInfoRequest { OrderId = OrderId, ip = clientIp }, username);    

            if(result == null || !result.Success)
            {
                return BadRequest(result.message);
            }

            return Ok(result.Value);
            
        }
        /// <summary>
        /// Get Return URL of VNPay payment 
        /// </summary>
        /// <returns></returns>
        [HttpGet("/ReturnUrl")]
        [AllowAnonymous]
        public async Task<IActionResult> ReturnVNPayPayment()
        {
            
            var querylists = HttpContext.Request.Query;
            var result = await _paymentService.ValidPayment(querylists);

            if(result == null || !result.Success)
            {
                return BadRequest(result.message);
            }
            return Ok(result.message);

        }
        [HttpPost("ShipCOD")]


        public async Task<IActionResult> ShipCOD(int OrderId)
        {
            var username = User.GetUserName();

            var result = await _paymentService.ShipCOD(username, OrderId);
            if(result == null || !result.Success)
            {
                return BadRequest(result.message);
            }
            return Ok(result.Value);

        }
        /// <summary>
        /// GetAll Payment exist (Only Admin)
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetAllPaymentDetail()
        {
            var result = await _unitOfWork.RepositoryPaymentDetail.GetAll();
            if(result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// User get Payment Detail 
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPaymentDetali(int OrderId)
        {
            var username = User.GetUserName();
            if(username == null)
            {
                return BadRequest("Header Token is invalid");
            }

            var result = await _paymentService.GetPaymentDetail(username,OrderId);

            if(result == null || !result.Success)
            {
                return BadRequest(result.message) ;
            }
            return Ok(result.Value);
        }
        /// <summary>
        /// For Admin want to edit StatusPayment
        /// </summary>
        /// <param name="paymentId"></param>
        /// <param name="paymentStatus"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> UpdatePaymentStatus(int paymentId,int paymentStatus,int StatusOrderId)
        {
            var paymentdetail = await _unitOfWork.RepositoryPaymentDetail.GetById(paymentId);
            if(paymentdetail == null)
            {
                return BadRequest("paymentId is not exist");
            }
            var order = _unitOfWork.RepositoryOrder.GetSingleByCondition(x => x.PaymentId == paymentId);
            var orderlog = _unitOfWork.RepositoryOrderStatusLog.Insert(new OrderStatusLog
            {
                LogAt = DateTime.Now,
                OrderId = order.Id,
                StatusId = StatusOrderId,
                TextLog = $"Order No.{order.Id} Update Status PaymentStatus at {DateTime.Now}"
            });
            paymentdetail.PaymentStatusId = paymentStatus;
            await _unitOfWork.CommitAsync();

            return Ok(paymentdetail);
        }

    }
}
