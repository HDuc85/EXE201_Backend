using Data.Models;
using Data.ViewModel;
using Data.ViewModel.Product;
using Microsoft.AspNetCore.Mvc;

namespace Service.Repo
{
    public interface IProductService
    {
        Task<Data.Models.Product> CreateProduct(CreateProductDTO createProductDto);
        Task<Product> UpdateProduct(int productId, UpdateProductDTO updateProductDto);
        Task<Product> GetProduct(int id);
        Task<ApiResult<bool>> DeleteProduct(int productid);
        Task<IEnumerable<Product>> GetProducts();
        Task<IEnumerable<Product>> SearchProductsByName(string productName);
        Task<IEnumerable<ProductDTO>> GetProductsbyTagValue(string tagValue);

              

        }
}
