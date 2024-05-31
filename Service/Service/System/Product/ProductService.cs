using Service.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using Service.Repo;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Service.ViewModel.System;
using Microsoft.EntityFrameworkCore;

namespace Service.Service.System.Product
{
    public class ProductService : IProductService
    {
        private PostgresContext _postgresContext;
        private readonly IConfiguration _config;
        public ProductService(PostgresContext postgresContext, IConfiguration config) {

            _postgresContext = postgresContext;
            _config = config;
        }


        Task<ActionResult<IEnumerable<Models.Product>>> IProductService.GetProducts()
        {
            throw new NotImplementedException();
        }

        public async Task<Models.Product> CreateProduct(CreateProductDTO createProductDto)
        {
            var product = new Models.Product
            {
                ProductName = createProductDto.ProductName,
                QuantitySold = createProductDto.QuantitySold,
                Rate = createProductDto.Rate,
                Description = createProductDto.Description,
                Auther = createProductDto.Auther,
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

                product.ProductVariants.Add(productVariant);
            }

            _postgresContext.Products.Add(product);
            await _postgresContext.SaveChangesAsync();

            return product;
        }

        private async Task<Size> GetOrCreateSizeAsync(string sizeName)
        {
            if (string.IsNullOrEmpty(sizeName)) return null;

            var size = await _postgresContext.Sizes.FirstOrDefaultAsync(s => s.SizeValue == sizeName);
            if (size == null)
            {
                size = new Size { SizeValue = sizeName, IsActive = true };
                _postgresContext.Sizes.Add(size);
                await _postgresContext.SaveChangesAsync();
            }
            return size;
        }

        private async Task<Brand> GetOrCreateBrandAsync(string brandName)
        {
            if (string.IsNullOrEmpty(brandName)) return null;

            var brand = await _postgresContext.Brands.FirstOrDefaultAsync(b => b.BrandValue == brandName);
            if (brand == null)
            {
                brand = new Brand { BrandValue = brandName, IsActive = true };
                _postgresContext.Brands.Add(brand);
                await _postgresContext.SaveChangesAsync();
            }
            return brand;
        }

        private async Task<Color> GetOrCreateColorAsync(string colorName)
        {
            if (string.IsNullOrEmpty(colorName)) return null;

            var color = await _postgresContext.Colors.FirstOrDefaultAsync(c => c.ColorValue == colorName);
            if (color == null)
            {
                color = new Color { ColorValue = colorName, IsActive = true };
                _postgresContext.Colors.Add(color);
                await _postgresContext.SaveChangesAsync();
            }
            return color;
        }

        public async Task<Models.Product> UpdateProduct(int productId, UpdateProductDTO updateProductDto)
        {
            var product = await _postgresContext.Products
                                .Include(p => p.ProductVariants)
                                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            // Update the product fields
            product.ProductName = updateProductDto.ProductName;
            product.QuantitySold = updateProductDto.QuantitySold;
            product.Rate = updateProductDto.Rate;
            product.Description = updateProductDto.Description;
            product.Auther = updateProductDto.Auther;

            // Remove existing product variants
            var existingVariants = product.ProductVariants.ToList();
            _postgresContext.ProductVariants.RemoveRange(existingVariants);
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
            _postgresContext.Products.Update(product);
            await _postgresContext.SaveChangesAsync();

            return product;
        }

        public async Task<Models.Product> GetProduct(int id)
        {
         
                var product = await _postgresContext.Products
                    .Where(p => p.Id == id)
                    .Select(p => new Models.Product
                    {
                        Id = p.Id,
                        ProductName = p.ProductName,
                        QuantitySold = p.QuantitySold,
                        Rate = p.Rate,
                        Description = p.Description,
                        Auther = p.Auther,
                        ProductVariants = p.ProductVariants.Select(pv => new Models.ProductVariant
                        {
                            Id = pv.Id,
                            ProductId = pv.ProductId,
                            SizeId = pv.Size.Id,
                            ColorId = pv.Color.Id,
                            BrandId = pv.Brand.Id, 
                            Thumbnail = pv.Thumbnail,
                            Price = pv.Price,
                            Quantity = pv.Quantity,
                            IsActive = pv.IsActive
                        }).ToList()
                    }).FirstOrDefaultAsync();

                return product;
            
        }
    }
}
