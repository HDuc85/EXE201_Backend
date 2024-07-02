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

using Data.ViewModel;
using static System.Net.Mime.MediaTypeNames;
using Data.ViewModel.User;
using Firebase.Auth;
using Service.Helper;
using Microsoft.EntityFrameworkCore;


using Service.Helper.Media;



namespace Service.Service.System.Product
{
    public class ProductService : IProductService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediaHelper _mediaHelper;

        public ProductService(UnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IMediaHelper mediaHelper)
        {

            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mediaHelper = mediaHelper;
        }



        public async Task<Data.Models.Product> CreateProduct(CreateProductDTO createProductDto)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var product = new Data.Models.Product
            {
                ProductName = createProductDto.ProductName,
                QuantitySold = createProductDto.QuantitySold,
                //Rate = createProductDto.Rate,
                Description = createProductDto.Description,
                Auther = Guid.Parse(userId),
                ProductVariants = new List<ProductVariant>(),
                ProductMedia = new List<ProductMedia>(),
                ProductTags = new List<ProductTag>(),
            };
            foreach (var variantDto in createProductDto.ProductVariants)
            {
                var size = await GetOrCreateSizeAsync(variantDto.SizeName);
                var brand = await GetOrCreateBrandAsync(variantDto.BrandName);
                var color = await GetOrCreateColorAsync(variantDto.ColorName);
                var saveimage = await _mediaHelper.SaveMediaBase64(variantDto.Thumbnail, "ProductVariant");
                var productVariant = new ProductVariant
                {
                    SizeId = size?.Id,
                    BrandId = brand?.Id,
                    ColorId = color?.Id,
                    Price = variantDto.Price,
                    Quantity = variantDto.Quantity,
                    Thumbnail = saveimage.url,
                    IsActive = true,
                };

                product.ProductVariants.Add(productVariant);
            }
            _unitOfWork.RepositoryProduct.Insert(product);
            await _unitOfWork.CommitAsync();
            if (createProductDto.MediaUrls != null && createProductDto.MediaUrls.Any())
            {
                foreach (var mediaUrl in createProductDto.MediaUrls)
                {
                    int mediaTypeId = IsVideo(mediaUrl) ? 1 : 2; // 1 for video, 2 for image
                    var media = new Media
                    {
                        MediaUrl = mediaUrl,
                        MediaTypeId = mediaTypeId,
                        IsActive = true
                    };
                    _unitOfWork.RepositoryMedia.Insert(media);
                    await _unitOfWork.CommitAsync(); // Save to get media ID


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
            var product = await _unitOfWork.RepositoryProduct.GetById(productId);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            // Update the product fields
            product.ProductName = updateProductDto.ProductName;
            product.Description = updateProductDto.Description;

            // Remove existing product variants
            var existingVariants = await _unitOfWork.RepositoryProductVariant.GetListByCondition(c => c.ProductId == productId);
            foreach (var variant in existingVariants)
            {
                if (!string.IsNullOrEmpty(variant.Thumbnail))
                {
                    await _mediaHelper.DeleteFileFromFirebase(variant.Thumbnail); // Implement this method
                }
            }
            _unitOfWork.RepositoryProductVariant.RemoveRange(existingVariants);
            // Add new product variants
            foreach (var variantDto in updateProductDto.ProductVariants)
            {
                var size = await GetOrCreateSizeAsync(variantDto.SizeName);
                var brand = await GetOrCreateBrandAsync(variantDto.BrandName);
                var color = await GetOrCreateColorAsync(variantDto.ColorName);
                var saveimage = await _mediaHelper.SaveMediaBase64(variantDto.Thumbnail, "ProductVariant");

                var productVariant = new ProductVariant
                {
                    ProductId = product.Id,
                    SizeId = size?.Id,
                    BrandId = brand?.Id,
                    ColorId = color?.Id,
                    Price = variantDto.Price,
                    Quantity = variantDto.Quantity,
                    Thumbnail = saveimage.url,
                    IsActive = true,
                };

                product.ProductVariants.Add(productVariant);
            }

            // Update media
            if (updateProductDto.MediaUrls != null && updateProductDto.MediaUrls.Any())
            {
                var existingMedia = await _unitOfWork.RepositoryProductMedia.GetListByCondition(m => m.ProductId == productId);
                _unitOfWork.RepositoryProductMedia.RemoveRange(existingMedia);

                foreach (var mediaUrl in updateProductDto.MediaUrls)
                {
                    int mediaTypeId = IsVideo(mediaUrl) ? 1 : 2;
                    var media = new Media
                    {
                        MediaUrl = mediaUrl,
                        MediaTypeId = mediaTypeId,
                        IsActive = true
                    };
                    _unitOfWork.RepositoryMedia.Insert(media);
                    await _unitOfWork.CommitAsync();

                    var productMedia = new ProductMedia
                    {
                        ProductId = product.Id,
                        MediaId = media.Id,
                        IsActive = true
                    };

                    _unitOfWork.RepositoryProductMedia.Insert(productMedia);
                }
            }

            // Update tags
            if (updateProductDto.TagValues != null && updateProductDto.TagValues.Any())
            {
                var existingTags = await _unitOfWork.RepositoryProductTag.GetListByCondition(t => t.ProductId == productId);
                _unitOfWork.RepositoryProductTag.RemoveRange(existingTags);

                foreach (var tagValueStr in updateProductDto.TagValues)
                {
                    var tagValue = await GetOrCreateTagValueAsync(tagValueStr);
                    var productTag = new ProductTag
                    {
                        ProductId = product.Id,
                        TagVauleId = tagValue.Id,
                        IsActive = true
                    };
                    product.ProductTags.Add(productTag);
                }
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
            var tags = await _unitOfWork.RepositoryProductTag
                .GetAll(pv => pv.ProductId == id);
            var media = await _unitOfWork.RepositoryProductMedia
                .GetAll(pv => pv.ProductId == id);
            product.ProductTags = tags.Select(pt => new ProductTag
            {
                TagVaule = pt.TagVaule,
                IsActive = true
            }).ToList();
            product.ProductMedia = media.Select(pm => new ProductMedia
            {
                MediaId = pm.MediaId,
                IsActive = true
            }).ToList();

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

        public async Task<ApiResult<bool>> DeleteProduct(int productid)
        {

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

        }
        public async Task<IEnumerable<ProductDTO>> GetProductsbyTagValue(string tagValue)
        {
            var query = _unitOfWork.RepositoryProduct.GetAllWithCondition()
                                                     .Include(x => x.ProductVariants)
                                                     .Include(y => y.ProductMedia)
                                                     .Include(z => z.ProductTags)
                                                     .ThenInclude(pt => pt.TagVaule)
                                                     .Where(p => p.ProductTags.Any(pt => pt.TagVaule.Value == tagValue && pt.IsActive == true));

            var products = await query.Select(p => new ProductDTO
            {
                ProductName = p.ProductName,
                QuantitySold = p.QuantitySold,
                Description = p.Description,
                Auther = p.Auther,
                ProductVariants = p.ProductVariants.Select(v => new ProductVariantDTO
                {
                    Thumbnail = v.Thumbnail,
                    Price = v.Price,
                    Quantity = v.Quantity,
                    IsActive = v.IsActive
                }).ToList(),
                ProductMedia = p.ProductMedia.Select(m => new ProductMediaDTO
                {
                    Id = m.Id,
                    ProductId = m.ProductId,
                }).ToList(),
                ProductTags = p.ProductTags.Select(t => new ProductTagDTO
                {
                    Id = t.Id,
                    ProductId = t.ProductId,
                }).ToList()
            }).ToListAsync();

            return products;
        }

    }
}
