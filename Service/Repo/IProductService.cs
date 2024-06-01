using Microsoft.AspNetCore.Mvc;
using Service.Models;
using Service.ViewModel.System;

namespace Service.Repo
{
    public interface IProductService
    {
        Task<Product> CreateProduct(CreateProductDTO createProductDto);
        Task<Product> UpdateProduct(int productId, UpdateProductDTO updateProductDto);
        Task<Product> GetProduct(int id);
        Task<ActionResult<IEnumerable<Product>>> GetProducts();
    }
}
