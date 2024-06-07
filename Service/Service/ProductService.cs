using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data.Models;
using Service.Interface;
using Service.Repo;
using Data.ViewModel.Product;

namespace Service.Service.System.Product
{
    public class ProductService : IProductService
    {
        private readonly UnitOfWork _unitOfWork;
        public ProductService(UnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
        }


        Task<ActionResult<IEnumerable<Data.Models.Product>>> IProductService.GetProducts()
        {
            throw new NotImplementedException();
        }

        public async Task<Data.Models.Product> CreateProduct(CreateProductDTO createProductDto)
        {
            var product = new Data.Models.Product
            {
                ProductName = createProductDto.ProductName,
                QuantitySold = createProductDto.QuantitySold,
                //Rate = createProductDto.Rate,
                Description = createProductDto.Description,
                ProductVariants = new List<ProductVariant>()
            };
            foreach (var variantDto in createProductDto.ProductVariants)
            {
                var size = await GetOrCreateSizeAsync(variantDto.SizeName);
                var brand = await GetOrCreateBrandAsync(variantDto.BrandName);
                var color = await GetOrCreateColorAsync(variantDto.ColorName);

                var productVariant = new ProductVariant
                {
                    SizeId = size?.Id,
                    BrandId = brand?.Id,
                    ColorId = color?.Id,
                    Price = variantDto.Price,
                    Quantity = variantDto.Quantity,
                    IsActive = true,
                };
                if (variantDto.Thumbnail != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await variantDto?.Thumbnail.CopyToAsync(memoryStream);
                        productVariant.Thumbnail = Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
                else
                {
                    productVariant.Thumbnail = null; // Set to null if thumbnail is null
                }

                product.ProductVariants.Add(productVariant);
            }

            _unitOfWork.RepositoryProduct.Insert(product);
            await _unitOfWork.CommitAsync();

            return product;
        }


        private async Task<Size> GetOrCreateSizeAsync(string sizeName)
        {
            if (string.IsNullOrEmpty(sizeName)) return null;

            var size = await _unitOfWork.RepositorySize.GetSingleByCondition(s => s.SizeValue == sizeName);
            if (size == null)
            {
                size = new Size { SizeValue = sizeName, IsActive = true };
                _unitOfWork.RepositorySize.Insert(size);
                await _unitOfWork.CommitAsync();
            }
            return size;
        }

        private async Task<Brand> GetOrCreateBrandAsync(string brandName)
        {
            if (string.IsNullOrEmpty(brandName)) return null;

            var brand = await _unitOfWork.RepositoryBrand.GetSingleByCondition(b => b.BrandValue == brandName);
            if (brand == null)
            {
                brand = new Brand { BrandValue = brandName, IsActive = true };
                _unitOfWork.RepositoryBrand.Insert(brand);
                await _unitOfWork.CommitAsync();
            }
            return brand;
        }

        private async Task<Color> GetOrCreateColorAsync(string colorName)
        {
            if (string.IsNullOrEmpty(colorName)) return null;

            var color = await _unitOfWork.RepositoryColor.GetSingleByCondition(c => c.ColorValue == colorName);
            if (color == null)
            {
                color = new Color { ColorValue = colorName, IsActive = true };
                _unitOfWork.RepositoryColor.Insert(color);
                await _unitOfWork.CommitAsync();
            }
            return color;
        }

        public async Task<Data.Models.Product> UpdateProduct(int productId, UpdateProductDTO updateProductDto)
        {
            //var product = await _postgresContext.Products
            //                    .Include(p => p.ProductVariants)
            //                    .FirstOrDefaultAsync(p => p.Id == productId);
            var product = await _unitOfWork.RepositoryProduct.GetById(productId);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            // Update the product fields
            product.ProductName = updateProductDto.ProductName;
            //product.QuantitySold = updateProductDto.QuantitySold;
            //product.Rate = updateProductDto.Rate;
            product.Description = updateProductDto.Description;
            //product.Auther = product.Auther;

            // Remove existing product variants
            var existingVariants = product.ProductVariants.ToList();
            _unitOfWork.RepositoryVariant.RemoveRange(existingVariants);
            product.ProductVariants.Clear();

            // Add new product variants
            foreach (var variantDto in updateProductDto.ProductVariants)
            {
                // Handle Size
                var size = await GetOrCreateSizeAsync(variantDto.SizeName);
                var brand = await GetOrCreateBrandAsync(variantDto.BrandName);
                var color = await GetOrCreateColorAsync(variantDto.ColorName);

                var productVariant = new ProductVariant
                {
                    ProductId = product.Id,
                    SizeId = size?.Id,
                    BrandId = brand?.Id,
                    ColorId = color?.Id,
                    Price = variantDto.Price,
                    Quantity = variantDto.Quantity,
                    IsActive = true,
                };

                product.ProductVariants.Add(productVariant);
            }

            // Save changes to the database
            _unitOfWork.RepositoryProduct.Update(product);
            await _unitOfWork.CommitAsync();

            return product;
        }

        public async Task<Data.Models.Product> GetProduct(int id)
        {

            var product = await _unitOfWork.RepositoryProduct
    .GetSingleByCondition(p => p.Id == id);

            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            // Fetch the related product variants using the UnitOfWork repository
            var productVariants = await _unitOfWork.RepositoryVariant
                .GetAll(pv => pv.ProductId == id);

            // Map the product variants to the desired format
            product.ProductVariants = productVariants.Select(pv => new ProductVariant
            {
                Id = pv.Id,
                ProductId = pv.ProductId,
                SizeId = pv.SizeId,
                ColorId = pv.ColorId,
                BrandId = pv.BrandId,
                Thumbnail = pv.Thumbnail,
                Price = pv.Price,
                Quantity = pv.Quantity,
                IsActive = pv.IsActive
            }).ToList();

            return product;

        }

        public Task<Data.Models.Product> DeleteProduct(int productid)
        {
            throw new NotImplementedException();
        }
    }
}
