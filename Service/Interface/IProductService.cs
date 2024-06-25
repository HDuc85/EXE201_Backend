using Data.Models;
using Data.ViewModel.Product;
using Microsoft.AspNetCore.Mvc;

namespace Service.Repo
{
    public interface IProductService
    {
<<<<<<< Updated upstream
        Task<Product> CreateProduct(CreateProductDTO createProductDto);
=======
        Task<Data.Models.Product> CreateProduct(CreateProductDTO createProductDto);
>>>>>>> Stashed changes
        Task<Product> UpdateProduct(int productId, UpdateProductDTO updateProductDto);
        Task<Product> GetProduct(int id);
        Task<Product> DeleteProduct(int productid);
        Task<ActionResult<IEnumerable<Product>>> GetProducts();
    }
}
