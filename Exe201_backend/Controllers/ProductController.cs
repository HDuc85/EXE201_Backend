using Data.Models;
using Data.ViewModel.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Interface;
using Service.Repo;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Exe201_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly PostgresContext _context;
        private readonly IProductService _productService;
        public ProductController(PostgresContext context, IProductService productService)
        {
            _context = context;
            _productService = productService;
        }
        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }
        // GET: api/Products{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productService.GetProduct(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Product>> CreateProduct([FromForm] CreateProductDTO createProductDto)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var id = GetUserIdFromToken(token);
            var product = await _productService.CreateProduct(createProductDto);
            if (id == Guid.Empty)
            {
                return Unauthorized();
            }

            product.Auther = id;
            return Ok(product);
        }
        private Guid GetUserIdFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Assuming the user's ID is stored in the "sub" claim
            var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
            return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) ? userId : Guid.Empty;
        }
        [HttpPut("UpdateProduct")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Product>> UpdateProduct(int productId, UpdateProductDTO updateProductDto)
{
            var product = await _productService.UpdateProduct(productId, updateProductDto);
            return Ok(product);
        }
        [HttpDelete("DeleteProduct")]
        public async Task<ActionResult<Product>> DeleteProduct(int productId)
        {
            var product = await _productService.DeleteProduct(productId);
            return Ok(product);
        }


    }
}
