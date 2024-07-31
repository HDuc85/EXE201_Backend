using Data.ViewModel.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Service.Helper.Header;
using Service.Interface;
using Service.Service;
using System.Data.Odbc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Exe201_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }


        /// <summary>
        /// Get All Order 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            string username = User.GetUserName();

            var result = await _orderService.GetAll(username);

            if (!result.Success)
            {
                return BadRequest(result.message);
            }
            return Ok(result.Value);
        }
        /// <summary>
        /// Get Order with ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetId")]
        [Authorize]
        public async Task<IActionResult> GetId(int id)
        {
            string username = User.GetUserName();

            var result = await _orderService.GetById(username,id);

            if (!result.Success)
            {
                return BadRequest(result.message);
            }
            return Ok(result.Value);
        }


        /// <summary>
        /// Check price service of shipping
        /// </summary>
        /// <param name="inforAddress"></param>
        /// <returns></returns>
        // GET: api/<OrderController>
        [HttpPost("GetPriceShip")]
        [Authorize]
        public async Task<IActionResult> GetPriceShip([FromBody]InforAddressDTO inforAddress)
        {
            var result = await _orderService.CheckShipPrice(inforAddress);
            return Ok(new ShipPriceResponse
            {
                ShipService = result.Item1,
                TotalWeight = result.Item2
            });
        }
        /// <summary>
        /// Make Order with PaymentType is 1 : VNPAY , 2 : COD
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MakeOrder([FromBody] MakeOrderDTO order)
        {
            string username = User.GetUserName();

            var result = await _orderService.MakeOrder(username,order);

            if(result.Value.PaymentURL != null && !result.Value.PaymentURL.IsNullOrEmpty())
            {
                Redirect(result.Value.PaymentURL);
            }
            if(result.Success)
            {
                return Ok(result.Value);

            }

            return BadRequest();

        }
        /// <summary>
        /// Update
        /// </summary>
        /// <param name="OrderId"></param>
        /// <param name="OrderStatusId"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus([FromBody]int OrderId, int OrderStatusId)
        {
            string username = User.GetUserName();

            var result = await _orderService.UpdateOrderStatus(username,OrderId,OrderStatusId);

            if (!result.Success)
            {
                return BadRequest(result.message);
            }
            return Ok(result.message);

        }
        /// <summary>
        /// Delete Order with Id
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete(int OrderId)
        {
            string username = User.GetUserName();

            var result = await _orderService.RemoveOrder(username, OrderId);
            if(result.Success)
            {
                return Ok(result.message);
            }
            return BadRequest();

        }
        /// <summary>
        /// To get Total Price Item Order with voucher
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        // POST api/<OrderController>
        [HttpPost("Total")]
        [Authorize]
        public async Task<IActionResult> TotalPriceItem([FromBody]TotalPriceItemDTO request)
        {
            string username = User.GetUserName();

            var result = await _orderService.TotalPriceItem(username, request);

            if(result.Success)
            {
                return Ok(result.Value);
            }
            return BadRequest(result.message);

        }
        [HttpPost("CheckAddress")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckAddress([FromBody]string request)
        {
        
            var result = await _orderService.CheckAddress(request);

            if (result)
            {
                return Ok("Address valid");
            }
            return BadRequest("Address invalid");

        }


    }
}
