using Data.Models;
using Data.ViewModel;
using Data.ViewModel.Cart;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Service.Interface;

namespace Service.Service
{
    public class CartService : ICartService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public CartService(UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }


        public async Task<ApiResult<bool>> AddCart(CreateCartRequest createCartRequest)
        {

            var user = await _userManager.FindByNameAsync(createCartRequest.Username);
            if (user == null)
            {
                return new()
                {
                    Success = false,
                    message = "User is not exits"
                };
            }
            var userCarts = await _unitOfWork.RepositoryCart.GetAll(x => x.UserId == user.Id);
            List<Cart> carts = new List<Cart>();
            if (createCartRequest.productVariants != null)
                if (createCartRequest.productVariants.Count() > 0)
                {
                    foreach (var productVariantRequest in createCartRequest.productVariants)
                    {
                        var product = await _unitOfWork.RepositoryProductVariant.GetById(productVariantRequest.ProductVarianItemId);
                        if (product == null) continue;

                        if (!(productVariantRequest.Quantity > 0)) continue;

                        if (userCarts.Any(x => x.ProductVariantId == productVariantRequest.ProductVarianItemId))
                        {
                            userCarts.FirstOrDefault(x => x.ProductVariantId == productVariantRequest.ProductVarianItemId).Quantity = productVariantRequest.Quantity;
                        }
                        else
                        {
                            if(productVariantRequest.Quantity > 0)
                            carts.Add(new Cart
                            {
                                UserId = user.Id,
                                ProductVariantId = productVariantRequest.ProductVarianItemId,
                                Quantity = productVariantRequest.Quantity,
                            });
                        }
                    }
                }
            if (createCartRequest.boxs != null)
                if (createCartRequest.boxs.Count() > 0)
                {
                    foreach (var boxRequest in createCartRequest.boxs)
                    {
                        var box = await _unitOfWork.RepositoryBox.GetById(boxRequest.BoxId);
                        if (box == null) continue;

                        if (!(boxRequest.Quantity > 0)) continue;

                        if (userCarts.Any(x => x.BoxId == boxRequest.BoxId))
                        {
                            userCarts.FirstOrDefault(x => x.BoxId == boxRequest.BoxId).Quantity = boxRequest.Quantity;
                        }
                        else
                        {
                            if (boxRequest.Quantity > 0)
                                carts.Add(new Cart
                            {
                                UserId = user.Id,
                                BoxId = boxRequest.BoxId,
                                Quantity = boxRequest.Quantity,
                            });
                        }

                    }
                }

            if (createCartRequest.boxs != null || createCartRequest.productVariants != null)
            {
                var (message, lcarts) = await CheckQuantity(carts);
                var (messageuser, usercart) = await CheckQuantity(userCarts.ToList());
                userCarts = usercart;

                if (carts.Count() > 0) await _unitOfWork.RepositoryCart.Insert(lcarts);
                try
                {
                    await _unitOfWork.CommitAsync();
                    return new() { Success = true, message = "Cart have been added" };

                }
                catch (Exception ex)
                {
                    throw new Exception(message: "something wrong");
                }
            }

            return new() { Success = false, message = "Nothing add to cart " };

        }

        public async Task<ApiResult<List<CartViewModel>>> GetCart(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return new()
                {
                    Success = false,
                    message = $"{username} is not exits"
                };
            }

            var lcarts = await _unitOfWork.RepositoryCart.GetAll(x => x.UserId == user.Id);
            if (lcarts.Count() == 0)
            {
                return new()
                {
                    Success = true,
                    message = "Cart empty"
                };
            }
            var (message, carts) = await CheckQuantity(lcarts.ToList());

            List<CartViewModel> cartUser = new List<CartViewModel>();

            foreach (var cart in carts)
            {
                if (cart.ProductVariantId != null)
                {
                    var pv = await _unitOfWork.RepositoryProductVariant.GetById(cart.ProductVariantId);

                    var product = await _unitOfWork.RepositoryProduct.GetById(pv.ProductId);

                    ProductVariantViewModel pvModel = null;


                    if (pv.SizeId != null)
                    {
                        if (pvModel == null) pvModel = new ProductVariantViewModel();
                        var size = await _unitOfWork.RepositorySize.GetById(pv.SizeId);
                        pvModel.size = size.SizeValue;
                    }
                    if (pv.ColorId != null)
                    {
                        if (pvModel == null) pvModel = new ProductVariantViewModel();
                        var color = await _unitOfWork.RepositoryColor.GetById(pv.ColorId);
                        pvModel.color = color.ColorValue;
                    }
                    if (pv.BrandId != null)
                    {
                        if (pvModel == null) pvModel = new ProductVariantViewModel();
                        var brand = await _unitOfWork.RepositoryBrand.GetById(pv.BrandId);
                        pvModel.brand = brand.BrandValue;
                    }

                    cartUser.Add(new CartViewModel
                    {
                        Type = "Product",
                        Name = product.ProductName,
                        Price = pv.Price,
                        productVariantViewModel = pvModel,
                        discount = pv.Discount,
                        Quantity = cart.Quantity,
                        thumbnail = pv.Thumbnail,
                    });

                }
                if (cart.BoxId != null)
                {
                    var box = await _unitOfWork.RepositoryBox.GetById(cart.BoxId);
                    if (box != null)
                    {

                        cartUser.Add(new CartViewModel
                        {
                            Type = "Box",
                            thumbnail = box.Thumbnail,
                            Quantity = cart.Quantity,
                            Name = box.BoxName,
                            Price = box.Price,
                            discount = box.Discount,
                        });
                    }
                }

            }



            return new()
            {
                message = message,
                Success = true,
                Value = cartUser
            };
        }

        public async Task<ApiResult<List<CartViewModel>>> GetCartBySize(CartBySizeRequest cartBySizeRequest)
        {
            var user = await _userManager.FindByNameAsync(cartBySizeRequest.Username);
            if (user == null)
            {
                return new()
                {
                    Success = false,
                    message = $"{cartBySizeRequest.Username} is not exits"
                };
            }

            var lcarts = await _unitOfWork.RepositoryCart.GetPageSize(x => x.UserId == user.Id,cartBySizeRequest.PageIndex,cartBySizeRequest.PageSize);
            if (lcarts.Count() == 0)
            {
                return new()
                {
                    Success = true,
                    message = "Cart empty"
                };
            }

            var (message, carts) = await CheckQuantity(lcarts.ToList());

            List<CartViewModel> cartUser = new List<CartViewModel>();

            foreach (var cart in carts)
            {
                if (cart.ProductVariantId != null)
                {
                    var pv = await _unitOfWork.RepositoryProductVariant.GetById(cart.ProductVariantId);

                    var product = await _unitOfWork.RepositoryProduct.GetById(pv.ProductId);

                    ProductVariantViewModel pvModel = null;


                    if (pv.SizeId != null)
                    {
                        if (pvModel == null) pvModel = new ProductVariantViewModel();
                        var size = await _unitOfWork.RepositorySize.GetById(pv.SizeId);
                        pvModel.size = size.SizeValue;
                    }
                    if (pv.ColorId != null)
                    {
                        if (pvModel == null) pvModel = new ProductVariantViewModel();
                        var color = await _unitOfWork.RepositoryColor.GetById(pv.ColorId);
                        pvModel.color = color.ColorValue;
                    }
                    if (pv.BrandId != null)
                    {
                        if (pvModel == null) pvModel = new ProductVariantViewModel();
                        var brand = await _unitOfWork.RepositoryBrand.GetById(pv.BrandId);
                        pvModel.brand = brand.BrandValue;
                    }

                    cartUser.Add(new CartViewModel
                    {
                        Type = "Product",
                        Name = product.ProductName,
                        Price = pv.Price,
                        productVariantViewModel = pvModel,
                        discount = pv.Discount,
                        Quantity = cart.Quantity,
                        thumbnail = pv.Thumbnail,
                    });

                }
                if (cart.BoxId != null)
                {
                    var box = await _unitOfWork.RepositoryBox.GetById(cart.BoxId);
                    if (box != null)
                    {

                        cartUser.Add(new CartViewModel
                        {
                            Type = "Box",
                            thumbnail = box.Thumbnail,
                            Quantity = cart.Quantity,
                            Name = box.BoxName,
                            Price = box.Price,
                            discount = box.Discount,
                        });
                    }
                }

            }



            return new()
            {
                message = message,
                Success = true,
                Value = cartUser
            };
        }
        public async Task<ApiResult<bool>> SingleAdd(SingleAddRequest singleAddRequest)
        {
            string message = string.Empty;

            var user = await _userManager.FindByNameAsync(singleAddRequest.Username);
            if (user == null)
            {
                return new()
                {
                    Success = false,
                    message = $"{singleAddRequest.Username} is not exits"
                };
            }

            if(singleAddRequest.Type != null)
            {
                if (singleAddRequest.Type == 1 && singleAddRequest.Quantity > 0)
                {
                    var pvId = await _unitOfWork.RepositoryCart.GetSingleByCondition(x => x.ProductVariantId == singleAddRequest.Id);
                    if (pvId != null)
                    {
                        var pv = await _unitOfWork.RepositoryProductVariant.GetById(pvId.ProductVariantId);
                        var product = await _unitOfWork.RepositoryProduct.GetById(pv.ProductId);
                        if (pv.Quantity >= singleAddRequest.Quantity)
                        { pvId.Quantity = singleAddRequest.Quantity;
                            message += $"{product.ProductName} with Sizeid:{pv.SizeId} / BrandId: {pv.BrandId}/ Color: {pv.ColorId} set quantity to {singleAddRequest.Quantity} \n";
                        }
                        else
                        {
                            pvId.Quantity = pv.Quantity;
                            message += $"{product.ProductName} with Sizeid:{pv.SizeId} / BrandId: {pv.BrandId}/ Color: {pv.ColorId} is have only {pv.Quantity}  \n";
                        }
                    }
                    else
                    {
                        var pv = await _unitOfWork.RepositoryProductVariant.GetById(singleAddRequest.Id);
                        var product = await _unitOfWork.RepositoryProduct.GetById(pv.ProductId);

                        if (pv != null)
                        {
                            if(pv.Quantity >= singleAddRequest.Quantity)
                            {
                                message += $"{product.ProductName } with Sizeid:{pv.SizeId} / BrandId: {pv.BrandId}/ Color: {pv.ColorId} added with quantity : {singleAddRequest.Quantity}";
                                    await _unitOfWork.RepositoryCart.Insert(new Cart { 
                                        ProductVariantId = singleAddRequest.Id,  
                                        Quantity = singleAddRequest.Quantity,
                                        UserId = user.Id
                                    });
                            }
                            else
                            {
                                await _unitOfWork.RepositoryCart.Insert(new Cart
                                {
                                    ProductVariantId = singleAddRequest.Id,
                                    Quantity = pv.Quantity,
                                    UserId = user.Id
                                });
                              
                                message += $"{product.ProductName} with Sizeid:{pv.SizeId} / BrandId: {pv.BrandId}/ Color: {pv.ColorId} is have only {pv.Quantity}  \n";

                            }

                        }
                    }
                }
                if (singleAddRequest.Type == 2 && singleAddRequest.Quantity > 0)
                {
                    var boxId = await _unitOfWork.RepositoryCart.GetSingleByCondition(x => x.BoxId == singleAddRequest.Id);
                    if (boxId != null)
                    {
                        var box = await _unitOfWork.RepositoryBox.GetById(boxId.BoxId);
                        var boxItems = await _unitOfWork.RepositoryBoxItem.GetAll(x => x.BoxId.Equals(box.Id));
                        int min = 0;
                        foreach (var boxItem in boxItems)
                        {
                            var pv = await _unitOfWork.RepositoryProductVariant.GetById(boxItem.ProductVariantId);
                            if (pv.Quantity < boxItem.Quantity)
                            {
                                min = pv.Quantity;
                            }

                        }
                        if (min > 0)
                        {
                            boxId.Quantity = min;
                            message += $"{box.BoxName} is have only {min} \n";
                        }
                        else
                        {
                            message += $"{box.BoxName} is set quantity to {singleAddRequest.Quantity}\n";

                            boxId.Quantity = singleAddRequest.Quantity;
                        }
                    }
                    else
                    {
                        var box = await _unitOfWork.RepositoryBox.GetById(singleAddRequest.Id);
                        var boxItems = await _unitOfWork.RepositoryBoxItem.GetAll(x => x.BoxId.Equals(box.Id));
                        int min = 0;
                        foreach (var boxItem in boxItems)
                        {
                            var pv = await _unitOfWork.RepositoryProductVariant.GetById(boxItem.ProductVariantId);
                            if (pv.Quantity < boxItem.Quantity)
                            {
                                min = pv.Quantity;
                            }

                        }
                        if (min > 0)
                        {
                            await _unitOfWork.RepositoryCart.Insert(new Cart
                            {
                                BoxId = box.Id,
                                Quantity = min,
                                UserId = user.Id
                            });
                            message += $"{box.BoxName} is have only {min} \n";
                        }
                        else
                        {
                            message += $"{box.BoxName} is added {singleAddRequest.Quantity} \n";
                            await _unitOfWork.RepositoryCart.Insert(new Cart
                            {
                                BoxId = box.Id,
                                Quantity = singleAddRequest.Quantity,
                                UserId = user.Id
                            });
                        }
                    }
                }
            }

            try
            {
                await _unitOfWork.CommitAsync();
                return new()
                {
                    Success = true,
                    message = message.IsNullOrEmpty()?"Nothing change!":message
                };
            }
            catch (Exception ex)
            {
                return new()
                {

                    Success = false,
                    message = ex.Message,
                };
            }

        }
        public async Task<ApiResult<bool>> UpdateCart(UpdateCartRequest updateCartRequest)
        {
            string message = string.Empty;
            var user = await _userManager.FindByNameAsync(updateCartRequest.Username);
            if (user == null)
            {
                return new()
                {
                    Success = false,
                    message = $"{updateCartRequest.Username} is not exits"
                };
            }

            if (updateCartRequest.Items != null)
                if (updateCartRequest.Items.Count > 0)
                {
                    foreach (var item in updateCartRequest.Items)
                    {
                        if (item.Type == 1 && item.Quantity > 0)
                        {
                            var pvId = await _unitOfWork.RepositoryCart.GetSingleByCondition(x => x.ProductVariantId == item.Id);
                            if (pvId != null)
                            {
                                var pv = await _unitOfWork.RepositoryProductVariant.GetById(pvId.ProductVariantId);
                                var product = await _unitOfWork.RepositoryProduct.GetById(pv.ProductId);
                                if (pv.Quantity >= item.Quantity)
                                { pvId.Quantity = item.Quantity; }
                                else
                                {
                                    pvId.Quantity = pv.Quantity;
                                    message += $"{product.ProductName} with Sizeid:{pv.SizeId} / BrandId: {pv.BrandId}/ Color: {pv.ColorId} is have only {pv.Quantity}  \n";
                                }
                            }
                        }
                        if (item.Type == 2 && item.Quantity > 0)
                        {
                            var boxId = await _unitOfWork.RepositoryCart.GetSingleByCondition(x => x.BoxId == item.Id);
                            if (boxId != null)
                            {
                                var box = await _unitOfWork.RepositoryBox.GetById(boxId.BoxId);
                                var boxItems = await _unitOfWork.RepositoryBoxItem.GetAll(x => x.BoxId.Equals(box.Id));
                                int min = 0;
                                foreach (var boxItem in boxItems)
                                {
                                    var pv = await _unitOfWork.RepositoryProductVariant.GetById(boxItem.ProductVariantId);
                                    if (pv.Quantity < boxItem.Quantity)
                                    {
                                        min = pv.Quantity;
                                    }

                                }
                                if (min > 0)
                                {
                                    boxId.Quantity = min;
                                    message += $"{box.BoxName} is have only {min} \n";
                                }
                                else
                                {
                                    boxId.Quantity = item.Quantity;
                                }

                            }
                        }
                    }
                }
            try
            {
                await _unitOfWork.CommitAsync();
                return new()
                {
                    Success = true,
                    message = message.IsNullOrEmpty() ? "Updated !" : message
                };
            }
            catch (Exception ex)
            {
                return new()
                {

                    Success = false,
                    message = ex.Message,
                };
            }

        }

        public async Task<ApiResult<int>> DeleteCart(DeleteCartRequest request)
        {

            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                return new()
                {
                    Success = false,
                    message = $"{request.Username} is not exits"
                };
            }

            if (request.ListProductVariantId != null)
                if (request.ListProductVariantId.Count > 0)
                    foreach (var productVariantId in request.ListProductVariantId)
                    {
                        _unitOfWork.RepositoryCart.Delete(x => x.UserId == user.Id && x.ProductVariantId == productVariantId);
                    }
            if (request.ListBoxId != null)
                if (request.ListBoxId.Count > 0)
                    foreach (var boxId in request.ListBoxId)
                    {
                        _unitOfWork.RepositoryCart.Delete(x => x.UserId == user.Id && x.BoxId == boxId);
                    }
            try
            {
                await _unitOfWork.CommitAsync();
                return new()
                {
                    Success = true,
                    message = "Deleted"
                };
            }
            catch (Exception ex)
            {
                return new()
                {

                    Success = false,
                    message = ex.Message,
                };
            }
        }

        public async Task<ApiResult<int>> DeleteAllCart(string Username)
        {
            var user = await _userManager.FindByNameAsync(Username);
            if (user == null)
            {
                return new()
                {
                    Success = false,
                    message = $"{Username} is not exits"
                };
            }

            var carts = await _unitOfWork.RepositoryCart.GetAll(x => x.UserId == user.Id);
            _unitOfWork.RepositoryCart.Delete(x => x.UserId == user.Id);
            try
            {
                await _unitOfWork.CommitAsync();
                return new()
                {
                    Value = carts.Count(),
                    Success = true,
                    message = $"Delete all cart of {user.UserName}"
                };
            }
            catch (Exception ex)
            {
                return new()
                {

                    Success = false,
                    message = ex.Message,
                };
            }
        }

        public async Task<(string, List<Cart>)> CheckQuantity(List<Cart> carts)
        {
            if (carts != null)
            {
                if (carts.Count > 0)
                {
                    string message = string.Empty;
                    foreach (var cart in carts)
                    {
                        if (cart.ProductVariantId.HasValue)
                        {
                            var productQuantity = await _unitOfWork.RepositoryProductVariant.GetById(cart.ProductVariantId);
                            if (productQuantity != null)
                            {
                                if (productQuantity.Quantity < cart.Quantity)
                                {
                                    cart.Quantity = productQuantity.Quantity;
                                    message += $"{productQuantity.Id} with Sizeid:{productQuantity.SizeId}/ BrandId:{productQuantity.BrandId}/ Color:{productQuantity.ColorId} is have only {productQuantity.Quantity} ";
                                }
                            }
                        }
                        if (cart.BoxId.HasValue)
                        {
                            var box = await _unitOfWork.RepositoryBox.GetById(cart.BoxId);

                            var boxItems = await _unitOfWork.RepositoryBoxItem.GetAll(x => x.BoxId.Equals(box.Id));
                            int min = 0;
                            foreach (var boxItem in boxItems)
                            {
                                var pv = await _unitOfWork.RepositoryProductVariant.GetById(boxItem.ProductVariantId);
                                if (pv.Quantity < boxItem.Quantity)
                                {
                                    min = pv.Quantity;
                                }

                            }
                            if (min > 0)
                            {
                                cart.Quantity = min;
                                message += $"{box.BoxName} is have only {min} \n";
                            }
                        }
                    }

                    return (message, carts);

                }
                return (null, null);
            }
            return (null, null);

        }


        
    }
}
