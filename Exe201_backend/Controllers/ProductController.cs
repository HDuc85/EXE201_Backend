using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Models;
using Service.Repo;
using Service.ViewModel.System;

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
        public async Task<ActionResult<Product>> CreateProduct(CreateProductDTO createProductDto)
        {
            var product = await _productService.CreateProduct(createProductDto);
            return Ok(product);
        }
        [HttpPut("UpdateProduct")]
public async Task<ActionResult<Product>> UpdateProduct(int productId, UpdateProductDTO updateProductDto)
{
            var product = await _productService.UpdateProduct(productId, updateProductDto);
            return Ok(product);
        }




    }
}
