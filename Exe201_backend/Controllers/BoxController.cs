using Data.Models;
using Data.ViewModel.Box;
using Data.ViewModel.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Helper;
using Service.Interface;

namespace Exe201_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoxController : ControllerBase
    {
        private readonly IBoxService _boxService;

        public BoxController(IBoxService boxService)
        {
            _boxService = boxService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBox(CreateBoxRequest request)
        {
            var result = await _boxService.CreateBox(request);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("{boxId}")]
        public async Task<IActionResult> GetBoxById(int boxId)
        {
            var result = await _boxService.GetBoxById(boxId);
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllBoxes()
        {
            var result = await _boxService.GetAllBoxes();
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateBox(UpdateBoxRequest request)
        {
            var result = await _boxService.UpdateBox(request);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("delete/{boxId}")]
        public async Task<IActionResult> DeleteBox(int boxId)
        {
            var result = await _boxService.DeleteBox(boxId);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

    }
}