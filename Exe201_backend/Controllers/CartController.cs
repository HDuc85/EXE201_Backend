using Data.Models;
using Data.ViewModel.Cart;
using Data.ViewModel.System;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service.Helper;
using Service.Interface;

namespace Exe201_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartService _cartService;

        public CartController(ICartService cartService, IUnitOfWork unitOfWork) {

            _cartService = cartService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Add product to Cart 
        /// </summary>
        /// <param name="createCartRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddCart([FromBody] CreateCartRequest createCartRequest)
        {
            var username = User.GetUserName();
            
            var result = await _cartService.AddCart( username, createCartRequest);
            if (!result.Success)
            {
                return BadRequest(result.message);
            }
            return Ok(result.message);

        }
        /// <summary>
        /// GetAll cart of user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]

        public async Task<IActionResult> GetCart ()
        {
            var username = User.GetUserName();

            var result = await _cartService.GetCart(username);

            if (!result.Success || result.Value == null)
            {
                return BadRequest(result.message);
            }
            return Ok(new { result.message , result.Value});
            
        }
        /// <summary>
        /// Get Cart with page index and size
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet("PageSize")]
        [Authorize]

        public async Task<IActionResult> GetCartPageSize(int PageIndex, int PageSize)
        {
            var username = User.GetUserName();
            if(PageIndex <= 0 || PageSize <= 0) 
            {
                return BadRequest("Page index or Size is not a negative number");
            }
            CartBySizeRequest request = new CartBySizeRequest{ Username = username, PageSize=PageSize,PageIndex = PageIndex};
            var result = await _cartService.GetCartBySize(request);

            if (!result.Success || result.Value == null)
            {
                return BadRequest(result.message);
            }
            return Ok(new { result.message, result.Value });

        }
        /// <summary>
        /// Add one item to cart 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("SingleAdd")]
        [Authorize]

        public async Task<IActionResult> SingleAdd(CartItem request)
        {
            var username = User.GetUserName();
            var result = await _cartService.SingleAdd(new SingleAddRequest { 
                Username = username,
                Id = request.Id,
                Quantity = request.Quantity,
                Type = request.Type
            });

            if (!result.Success)
            {
                return BadRequest(result.message);
            }
            return Ok(result.message);

        }
        [HttpPut("Update")]
        [Authorize]

        public async Task<IActionResult> UpdateCart([FromBody] List<CartItem> updateCartRequest)
        {

            var username = User.GetUserName();
            
            foreach (var item in updateCartRequest)
            {
                if(item.Type <= 0 || item.Type > 2)
                {
                    return BadRequest($"Type of item : {item.Type} is valid");
                }
                if(item.Id <= 0)
                {
                    return BadRequest($"Item id : {item.Id} is valid");
                }
                if(item.Quantity < 0)
                {
                    return BadRequest($"{item.Quantity} is not right quantity");
                }
            }
            
            
            var result = await _cartService.UpdateCart(new UpdateCartRequest
            {
                Username=username,
                Items = updateCartRequest
            });

            if (!result.Success)
            {
                return BadRequest(result.message);
            }
            return Ok(result.message);

        }
        /// <summary>
        /// Delete array item in cart
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete("ListItem")]
        [Authorize]

        public async Task<IActionResult> DeleteCart(DeleteCartRequest request)
        {
            var username = User.GetUserName();
            var result = await _cartService.DeleteCart(username, request);

            if (!result.Success)
            {
                return BadRequest(result.message);
            }
            return Ok(result.message);

        }

        /// <summary>
        /// Delete all item in Cart
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]

        public async Task<IActionResult> DeleteAllCart()
        {
            var username = User.GetUserName();
            var result = await _cartService.DeleteAllCart(username);

            if (!result.Success)
            {
                return BadRequest(result.message);
            }
            return Ok(result.message);

        }

    }
}
