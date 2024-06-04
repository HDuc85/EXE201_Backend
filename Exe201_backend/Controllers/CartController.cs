using Data.Models;
using Data.ViewModel.Cart;
using Data.ViewModel.System;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace Exe201_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartService _cartService;
        
        public CartController(ICartService cartService,IUnitOfWork unitOfWork) { 
           
            _cartService = cartService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Add product to Cart 
        /// </summary>
        /// <param name="createCartRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddCart([FromBody] CreateCartRequest createCartRequest)
        {
            var result = await  _cartService.AddCart(createCartRequest);
            if(!result.Success) 
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
        [AllowAnonymous]
        public async Task<IActionResult> GetCart (string username)
        {
            var result = await _cartService.GetCart(username);

            if (!result.Success || result.Value == null)
            {
                return BadRequest(result.message);
            }
            return Ok(new { result.message , result.Value});
            
        }
        /// <summary>
        /// Add one item to cart 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("SingleAdd")]
        [AllowAnonymous]
        public async Task<IActionResult> SingleAdd(SingleAddRequest request)
        {
            var result = await _cartService.SingleAdd(request);

            if (!result.Success)
            {
                return BadRequest(result.message);
            }
            return Ok(result.message);

        }
        [HttpPut("Update")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateCart([FromBody] UpdateCartRequest updateCartRequest)
        {
          
            foreach(var item in updateCartRequest.Items)
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
            
            
            var result = await _cartService.UpdateCart(updateCartRequest);

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
        [AllowAnonymous]
        public async Task<IActionResult> DeleteCart(DeleteCartRequest request)
        {
            var result = await _cartService.DeleteCart(request);

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
        [AllowAnonymous]
        public async Task<IActionResult> DeleteAllCart(string Username)
        {
            var result = await _cartService.DeleteAllCart(Username);

            if (!result.Success)
            {
                return BadRequest(result.message);
            }
            return Ok(result.message);

        }

    }
}
