using Data.ViewModel.Analytic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Helper.Header;
using Service.Interface;
using Service.Service;

namespace Exe201_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticController : ControllerBase
    {
        private readonly IAnalyticService _analyticService;

        public AnalyticController(IAnalyticService analyticService){
            _analyticService = analyticService;
        }

        /// <summary>
        /// Get Total Users  
        /// </summary>
        /// <returns></returns>
        [HttpGet("UserCount")]
       // [Authorize(Roles ="admin")]
        public async Task<IActionResult> UserCount()
        {

            var result = await _analyticService.UserCount();
            return Ok(result);
        }

        [HttpGet("ProductCount")] 
        public async Task<IActionResult> ProductCount(){
            var result = await _analyticService.ProductCount();
                return Ok(result);
        }
        [HttpGet("OrderCount")] 
        public async Task<IActionResult> OrderCount(){
        var result = await _analyticService.OrderCount();
                return Ok(result);
        }
        [HttpGet("TotalPriceCount")] 
        public async Task<IActionResult> TotalPriceCount(){
        var result = await _analyticService.TotalPriceCount();
                return Ok(result);
        }
        [HttpGet("topBuyProduct")] 
        public async Task<IActionResult> topBuyProduct(){
        var result = await _analyticService.topBuyProduct();
                return Ok(result);
        }
        [HttpGet("topUserBuy")]
        public async Task<IActionResult> topUserBuy() { 
        var result = await _analyticService.topUserBuy();
                return Ok(result);
        }
    }
}
