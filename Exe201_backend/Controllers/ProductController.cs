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
            var products = await _productService.GetProducts();
            if (products == null)
            {
                return NotFound();
            }
            return Ok(products);

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
            var product = await _productService.CreateProduct(createProductDto);
            return Ok(product);
        }
        [HttpPut("UpdateProduct")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Product>> UpdateProduct(int productId, UpdateProductDTO updateProductDto)
        {
            var product = await _productService.UpdateProduct(productId, updateProductDto);
            return Ok(product);
        }
        [HttpDelete("DeleteProduct/{productId}")]
        public async Task<ActionResult> DeleteProduct(int productId)
        {
            var result = await _productService.DeleteProduct(productId);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpGet("SearchProductsbyName{productname}")]
        public async Task<ActionResult<Product>> SearchProductsbyName(string productname)
        {
            var products = await _productService.SearchProductsByName(productname);

            if (products == null || !products.Any())
            {
                return NotFound("No products found with the specified name.");
            }

            return Ok(products);

        }

    }
}
