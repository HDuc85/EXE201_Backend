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
<<<<<<< Updated upstream
=======
using Data.ViewModel;
using static System.Net.Mime.MediaTypeNames;
using Data.ViewModel.User;
using Firebase.Auth;
using Service.Helper;
using Microsoft.EntityFrameworkCore;

>>>>>>> Stashed changes

namespace Service.Service.System.Product
{
    public class ProductService : IProductService
    {
        private readonly UnitOfWork _unitOfWork;
        public ProductService(UnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
        }


<<<<<<< Updated upstream
        Task<ActionResult<IEnumerable<Data.Models.Product>>> IProductService.GetProducts()
        {
            throw new NotImplementedException();
        }
=======
>>>>>>> Stashed changes

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

<<<<<<< Updated upstream
=======
                    var productMedia = new ProductMedia
                    {
                        ProductId = product.Id,
                        MediaId = media.Id,
                        IsActive = true
                    };

                    _unitOfWork.RepositoryProductMedia.Insert(productMedia);
                    //product.ProductMedia.Add(productMedia);
                }
            }
            if (createProductDto.TagValues != null && createProductDto.TagValues.Any())
            {
                foreach (var tagValueStr in createProductDto.TagValues)
                {
                    var tagValue = await GetOrCreateTagValueAsync(tagValueStr); // Hàm lấy hoặc tạo tag value
                    var productTag = new ProductTag
                    {
                        ProductId = product.Id,
                        TagVauleId = tagValue.Id,
                        IsActive = true
                    };
                    product.ProductTags.Add(productTag);
                }
            }
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
            throw new NotImplementedException();
=======
            var product = await _unitOfWork.RepositoryProduct.GetById(productid);

            // Kiểm tra nếu sản phẩm không tồn tại
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            // Xóa các biến thể của sản phẩm trước
            var productVariants = await _unitOfWork.RepositoryVariant.GetListByCondition(pv => pv.ProductId == productid);
            if (productVariants.Any())
            {
                _unitOfWork.RepositoryVariant.RemoveRange(productVariants);
            }

            // Xóa sản phẩm
            _unitOfWork.RepositoryProduct.Delete(product);

            // Lưu các thay đổi vào cơ sở dữ liệu
            await _unitOfWork.CommitAsync();

            return new ApiResult<bool> { Success = true, message = "Product deleted successfully" };
        }

        public async Task<IEnumerable<Data.Models.Product>> GetProducts()
        {
            var query = _unitOfWork.RepositoryProduct.GetAllWithCondition()
                                                     .Include(x => x.ProductVariants)
                                                     .Include(y => y.ProductMedia)
                                                     .Include(z => z.ProductTags);

            var products = await query.ToListAsync();
            return products;
        }

        private bool IsVideo(string url)
        {
            var videoExtensions = new List<string> { ".mp4", ".avi", ".mov", ".wmv", ".flv" };
            return videoExtensions.Any(ext => url.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }
        public async Task<IEnumerable<Data.Models.Product>> SearchProductsByName(string productName)
        {
            var products = _unitOfWork.RepositoryProduct.GetAllWithCondition(p => p.ProductName.Contains(productName))
                                                        .Include(p => p.ProductVariants)
                                                        .Include(p => p.ProductMedia)
                                                        .Include(p => p.ProductTags);
            var productList = await products.ToListAsync();
            return productList ?? new List<Data.Models.Product>();
        }

        private async Task<TagValue> GetOrCreateTagValueAsync(string value)
        {
            var existingTagValue = await _unitOfWork.RepositoryTagValue.GetSingleByCondition(tv => tv.Value == value);
            if (existingTagValue != null)
            {
                return existingTagValue;
            }
            var newTagValue = new TagValue { Value = value };
            _unitOfWork.RepositoryTagValue.Insert(newTagValue);
            await _unitOfWork.CommitAsync();
            return newTagValue;
>>>>>>> Stashed changes
        }
    }
}
