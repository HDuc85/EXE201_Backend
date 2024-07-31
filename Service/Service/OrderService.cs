using Data.Models;
using Data.ViewModel;
using Data.ViewModel.Cart;
using Data.ViewModel.Order;
using Data.ViewModel.Payment;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Service.Helper.Address;
using Service.Interface;
using System.Collections;
using System.Text;

namespace Service.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ITokenHandler _tokenHandler;
        private readonly IAddressHelper _addressHelper;
        private readonly IConfiguration _configuration;
        private readonly ICartService _cartService;
        private readonly IPaymentService _paymentService;
        public OrderService(IUnitOfWork unitOfWork,
            IUserService userService,
            ITokenHandler tokenHandler,
            IAddressHelper addressHelper,
            IConfiguration configuration,
            ICartService cartService,
            IPaymentService paymentService)
        {
            _cartService = cartService;
            _configuration = configuration;
            _addressHelper = addressHelper;
            _unitOfWork = unitOfWork;
            _userService = userService;
            _tokenHandler = tokenHandler;
            _paymentService = paymentService;
        }
        /* ORDER_PAYMENT
            1: Uncollect money
            2: Collect express fee and price of goods.
            3: Collect price of goods
            4: Collect express fee
         */

        public async Task<bool> CheckAddress(string inforAddress)
        {
            var store = await _unitOfWork.repositoryStore.GetSingleByCondition();
            var senderAddress = await _addressHelper.AddressFormater(store.Address);



            int? weight = 100;

            var payload = new
            {
                SENDER_ADDRESS = $"{senderAddress}",
                RECEIVER_ADDRESS = $"{inforAddress}",
                PRODUCT_TYPE = "HH",
                PRODUCT_WEIGHT = weight,
                TYPE = 1
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var token = await _tokenHandler.GetTokenVTPAsync();
            string getpriceUrl = _configuration["VTP:GetPriceUrl"];

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("token", token);

                var response = await client.PostAsync(getpriceUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Request failed with status code: {response.StatusCode}");
                }
                var responseString = await response.Content.ReadAsStringAsync();
                JObject jsonResponse = JObject.Parse(responseString);
                var result = jsonResponse?["status"];
                if (result == null)
                {
                return true;

                }
                else
                {
                    return false;
                }
            }


        }

        public async Task<(Object, int)> CheckShipPrice(InforAddressDTO inforAddress)
        {
            var store = await _unitOfWork.repositoryStore.GetSingleByCondition();
            var senderAddress = await _addressHelper.AddressFormater(store.Address);
            var receiverAddress = await _addressHelper.AddressFormater(inforAddress.ReceiverAddress);



            int? weight = 0;
            foreach (var item in inforAddress.ListCartItem)
            {
                if (item.Type == 1)
                {
                    var productvar = await _unitOfWork.RepositoryProductVariant.GetById(item.Id);
                    weight += productvar.weight * item.Quantity;
                }
                if (item.Type == 2)
                {
                    var boxItem = await _unitOfWork.RepositoryBoxItem.GetAll(x => x.BoxId == item.Id);
                    if (boxItem != null)
                        if (boxItem.Count() > 0)
                        {
                            foreach (var itembox in boxItem)
                            {
                                var iteminbox = await _unitOfWork.RepositoryProductVariant.GetById(itembox.ProductVariantId);
                                weight += iteminbox.weight * iteminbox.Quantity;
                            }
                        }
                }
            }


            if (weight == 0)
            {
                throw new Exception("Product in order is not have weight");
            }


            var payload = new
            {
                SENDER_ADDRESS = $"{senderAddress}",
                RECEIVER_ADDRESS = $"{receiverAddress}",
                PRODUCT_TYPE = "HH",
                PRODUCT_WEIGHT = weight,
                TYPE = 1
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var token = await _tokenHandler.GetTokenVTPAsync();
            string getpriceUrl = _configuration["VTP:GetPriceUrl"];

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("token", token);

                var response = await client.PostAsync(getpriceUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Request failed with status code: {response.StatusCode}");
                }
                var responseString = await response.Content.ReadAsStringAsync();
                JObject jsonResponse = JObject.Parse(responseString);
                var result = jsonResponse["RESULT"].ToString();

                List<OrderFeeShipServiceDTO> orderFeeShipServices = JsonConvert.DeserializeObject<List<OrderFeeShipServiceDTO>>(result);
                return (orderFeeShipServices, (int)weight);
            }


        }
        public async Task<ApiResult<bool>> UpdateOrderStatus(string username,int OrderId, int OrderStatusId)
        {
            var user = await _userService.UserExits(username);
            var order = await _unitOfWork.RepositoryOrder.GetSingleByCondition(x => x.UserId == user.Id && x.Id == OrderId);
            if (order == null)
            {
                return new()
                {
                    Success = false,
                    message = "Order is invalid"
                };
            }

            var orderStatus = await _unitOfWork.RepositoryOrderStatus.GetById(OrderStatusId); 
            if (orderStatus == null)
            {
                return new ApiResult<bool>()
                {
                    Success = false,
                    message = "StatusId is invalid"
                };
            }

            order.StatusId = OrderStatusId;

            try
            {
                await _unitOfWork.RepositoryOrderStatusLog.Insert(new OrderStatusLog
                {
                    LogAt = DateTime.Now,
                    OrderId = order.Id,
                    StatusId = OrderStatusId,
                    TextLog = $"Order No.{order.Id} Deleted at {DateTime.Now}"
                });
                await _unitOfWork.CommitAsync();
                return new ApiResult<bool>
                {
                    Success = true,
                    message = $"Update OrderId {order.Id} is successful"
                };

            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }
        public async Task<ApiResult<OrderViewDTO>> GetById(string username, int OrderId)
        {
            var user = await _userService.UserExits(username);
            var order = await _unitOfWork.RepositoryOrder.GetSingleByCondition(x => x.UserId == user.Id && x.Id == OrderId);
            if(order == null)
            {
                return new ApiResult<OrderViewDTO>() { Success = false, message = "Order is invalid" };
            }
            var status = await _unitOfWork.RepositoryOrderStatus.GetById(order.StatusId);
            var paymentdetail = await _unitOfWork.RepositoryPaymentDetail.GetById(order.PaymentId);
            var paymentSatus = await _unitOfWork.RepositoryPaymentStatus.GetById(paymentdetail.PaymentStatusId);
            var orderview = new OrderViewDTO()
            {
                OrderId = order.Id,
                TrackingNumber = order.TrackingNumber,
                ShipPrice = (double)order.ShipPrice,
                OrderStatus = status.Status,
                PaymentStatus = paymentSatus.Status,
                VoucherName = ""

            };
            if (order.VoucherId != null)
            {
                var voucher = await _unitOfWork.repositoryVoucher.GetById(order.VoucherId);
                if (voucher != null)
                {
                    orderview.VoucherName = voucher.VoucherName;
                }
            }

            return new ApiResult<OrderViewDTO>()
            {
                Success = true,
                Value = orderview
            };
            
        }
        public async Task<ApiResult<IEnumerable<OrderViewDTO>>> GetAll(string username)
        {
            var user = await _userService.UserExits(username);
            var listOder = await _unitOfWork.RepositoryOrder.GetAll(x => x.UserId == user.Id);

            if(listOder != null && listOder.Count() > 0)
            {
                var listPaymentSatus = await _unitOfWork.RepositoryPaymentStatus.GetAll();
                var listOrderStatus = await _unitOfWork.RepositoryOrderStatus.GetAll();
                List<OrderViewDTO> orderview = new List<OrderViewDTO>();
               
                foreach (var order in listOder)
                {
                    var paymentDetail = await _unitOfWork.RepositoryPaymentDetail.GetById(order.PaymentId);
                    if (paymentDetail != null)
                    {


                        string vourchername = string.Empty;
                        if (order.VoucherId != null)
                        {
                            var voucher = await _unitOfWork.repositoryVoucher.GetById(order.VoucherId);
                            if (voucher != null)
                            {
                                vourchername = voucher.VoucherName;
                            }
                        }


                        var orderstatus = listOrderStatus.FirstOrDefault(x => x.Id == order.StatusId);
                        var PaymentStatus = listPaymentSatus.FirstOrDefault(x => x.Id == paymentDetail.PaymentStatusId);
                        orderview.Add(new()
                        {
                            OrderId = order.Id,
                            ShipPrice = order.ShipPrice.HasValue ? (double) order.ShipPrice : 0,
                            TrackingNumber = order.TrackingNumber,
                            OrderStatus = orderstatus == null ? "" : orderstatus.Status,
                            PaymentStatus = PaymentStatus == null ? "" : PaymentStatus.Status,
                            VoucherName = vourchername
                        });
                    }
                }

                return new()
                {
                    Success = true,
                    Value = orderview
                };

            }
            return new () { Success = false, message = "Order is empty" };
        


         }
        public async Task<ApiResult<TotalPriceDTO>> TotalPriceItem(string username, TotalPriceItemDTO makeOrder)
        {
            var user = await _userService.UserExits(username);

            var listCart = await _unitOfWork.RepositoryCart.GetAll(x => x.UserId == user.Id);
            (var message, listCart) = await _cartService.CheckQuantity(listCart);
            var result = new TotalPriceDTO
            {
                TotalPrice = 0,
                ItemOrders = new List<ItemOrder>()
            };

            foreach (var item in makeOrder.ListCartItem)
            {
                if (item.Type == 1)
                {

                    var product = listCart.FirstOrDefault(x => x.ProductVariantId == item.Id);

                    if (product != null)
                    {
                        List<Data.ViewModel.Order.Type> types = new List<Data.ViewModel.Order.Type>();

                        var Productitem = await _unitOfWork.RepositoryProductVariant.GetById(item.Id);
                        var productmother = await _unitOfWork.RepositoryProduct.GetById(Productitem.ProductId);
                        if (Productitem.BrandId != null)
                        {
                            var brand = await _unitOfWork.RepositoryBrand.GetById(Productitem.BrandId);
                            types.Add(new()
                            {
                                Name = "Brand",
                                Description = brand.BrandValue,
                            });
                        }
                        if (Productitem.ColorId != null)
                        {
                            var color = await _unitOfWork.RepositoryColor.GetById(Productitem.ColorId);
                            types.Add(new()
                            {
                                Name = "Color",
                                Description = color.ColorValue,
                            });
                        }
                        if (Productitem.SizeId != null)
                        {
                            var size = await _unitOfWork.RepositorySize.GetById(Productitem.SizeId);
                            types.Add(new()
                            {
                                Name = "Size",
                                Description = size.SizeValue,
                            });
                        }
                        result.ItemOrders.Add(new ItemOrder
                        {
                            ItemName = productmother.ProductName,
                            ItemDescription = types,
                            ItemType = 1,
                            Price = (double)Productitem.Price,
                            Quantity = (int)item.Quantity,
                            ItemUrl = Productitem.Thumbnail,
                        });
                    }
                }
                if (item.Type == 2)
                {
                    var boxcart = listCart.FirstOrDefault(x => x.BoxId == item.Id);

                    if (boxcart != null)
                    {
                        var box = await _unitOfWork.RepositoryBox.GetById(item.Id);

                        result.ItemOrders.Add(new ItemOrder
                        {
                            ItemName = box.BoxName,
                            ItemType = 2,
                            Price = (double)box.Price,
                            Quantity = (int)item.Quantity,
                            ItemUrl = box.Thumbnail,
                        });
                    }
                }
            }

            result.TotalPrice = result.ItemOrders.Sum(order => order.Price * order.Quantity);
            if (makeOrder.voucherId != null)
            {
                var voucher = await _unitOfWork.repositoryVoucher.GetSingleByCondition(x => x.Id == makeOrder.voucherId);
                if (voucher != null)
                    result.TotalPrice = DiscountVoucher(result.TotalPrice, voucher);
            }

            return new()
            {
                Success = true,
                Value = result
            };

        }
        public async Task<ApiResult<bool>> RemoveOrder(string username, int? orderId)
        {
            var user = await _userService.UserExits(username);

            var order = await _unitOfWork.RepositoryOrder.GetById(orderId);

            if(order == null)
            {
                throw new Exception("Order Id is invalid");
            }

            if(order.TrackingNumber != null)
            {


            var payload = new
            {
                TYPE = 4,
                ORDER_NUMBER = order.TrackingNumber,
                NOTE = $"Cancel order number {order.Id}"
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var token = await _tokenHandler.GetTokenVTPAsync();
            string getpriceUrl = _configuration["VTP:CancelOrderUrl"];

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("token", token);

                var response = await client.PostAsync(getpriceUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Request failed with status code: {response.StatusCode}");
                }
               
            }

            }
            _unitOfWork.RepositoryOrderItem.Delete(x => x.OrderId == orderId);
           

            order.StatusId = 7;
            _unitOfWork.RepositoryOrder.Update(order);



            try
            {
                await _unitOfWork.RepositoryOrderStatusLog.Insert(new OrderStatusLog
                {
                    LogAt = DateTime.Now,
                    OrderId = orderId,
                    StatusId = 7,
                    TextLog = $"Order No.{orderId} Deleted at {DateTime.Now}"
                });
                await _unitOfWork.CommitAsync();
                return new()
                {
                    Success = true,
                    message = "Delete success"
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public async Task<ApiResult<MakeOrderReponseDTO>> MakeOrder(string username, MakeOrderDTO makeOrder)
        {
            var user = await _userService.UserExits(username);

            var store = await _unitOfWork.repositoryStore.
                                GetSingleByCondition();
           
            var receiverAddress = await _addressHelper.AddressFormater(makeOrder.ReceiverAddress);
            var senderAddress = await _addressHelper.AddressFormater(store.Address);

            string paymentUrl = string.Empty;
            var listItem = new List<LIST_ITEM_PAYLOAD>();
            var listCart = await _unitOfWork.RepositoryCart.GetAll(x => x.UserId == user.Id);
            List<OrderItem> ListorderItems = new List<OrderItem>();
            List<Cart> listCartorder = new List<Cart>();
            Order newOrder = new Order()
            {
                UserId = user.Id,
               // Price = makeOrder.totalPrice,
                StatusId = 1,
            };
            await _unitOfWork.RepositoryOrder.Insert(newOrder);

            await _unitOfWork.CommitAsync();
            double totalPrice = 0;
            int totalWeight = 0;
            foreach (var item in makeOrder.ListCartItem)
            {
                if (item.Type == 1)
                {
                    var product = listCart.FirstOrDefault(x => x.ProductVariantId == item.Id);
                    if (product != null)
                    {
                        var Productitem = await _unitOfWork.RepositoryProductVariant.GetById(item.Id);
                        var productmother = await _unitOfWork.RepositoryProduct.GetById(Productitem.ProductId);
                        var type = string.Empty;
                        if (Productitem.BrandId != null)
                        {
                            var brand = await _unitOfWork.RepositoryBrand.GetById(Productitem.BrandId);
                            type += "Brand : " + brand.BrandValue.ToString() + " ";
                        }
                        if (Productitem.ColorId != null)
                        {
                            var color = await _unitOfWork.RepositoryColor.GetById(Productitem.ColorId);
                            type += "Color : " + color.ColorValue.ToString() + " ";
                        }
                        if (Productitem.SizeId != null)
                        {
                            var size = await _unitOfWork.RepositorySize.GetById(Productitem.SizeId);
                            type += "Size : " + size.SizeValue.ToString() + " ";
                        }

                        ListorderItems.Add(new OrderItem
                        {
                            ProductVariantId = Productitem.Id,
                            OrderId = newOrder.Id,
                            Quantity = item.Quantity,
                            Price = Productitem.Discount != null ? PriceDiscount((double)Productitem.Price, (double)Productitem.Discount) : Productitem.Price,
                        });
                        listItem.Add(new()
                        {
                            PRODUCT_NAME = productmother.ProductName + " " + type,
                            PRODUCT_PRICE = (int)Productitem.Price,
                            PRODUCT_QUANTITY = item.Quantity
                        });
                        totalPrice += (int)Productitem.Price * item.Quantity;
                        if(Productitem.weight != null) 
                        totalWeight += (int)Productitem.weight;
                        listCartorder.Add(product);
                    }
                }
                if (item.Type == 2)
                {
                    var boxcart = listCart.FirstOrDefault(x => x.BoxId == item.Id);

                    if (boxcart != null)
                    {
                        var box = await _unitOfWork.RepositoryBox.GetById(item.Id);

                        ListorderItems.Add(new OrderItem
                        {
                            BoxId = box.Id,
                            OrderId = newOrder.Id,
                            Quantity = item.Quantity,
                            Price = box.Discount != null ? PriceDiscount((double)box.Price,(double)box.Discount) : box.Price,
                        });

                        listItem.Add(new()
                        {
                            PRODUCT_NAME = box.BoxName,
                            PRODUCT_PRICE = (int)box.Price,
                            PRODUCT_QUANTITY = item.Quantity
                        });
                        totalPrice += (double)box.Price * item.Quantity;
                        totalWeight += 100;
                        listCartorder.Add(boxcart);
                    }
                }

            }
            await _unitOfWork.RepositoryOrderItem.Insert(ListorderItems);
            await _unitOfWork.RepositoryOrderStatusLog.Insert(new OrderStatusLog
            {
                LogAt = DateTime.Now,
                OrderId = newOrder.Id,
                StatusId = newOrder.StatusId,
                TextLog = $"Order No.{newOrder.Id} Create at {DateTime.Now}"
            });

           
            
            

            var payload = new
            {
                ORDER_NUMBER = newOrder.Id,
                SENDER_FULLNAME = store.StoreName,
                SENDER_ADDRESS = senderAddress,
                SENDER_PHONE = store.Phone,
                RECEIVER_FULLNAME = makeOrder.ReceiverName,
                RECEIVER_ADDRESS = receiverAddress,
                RECEIVER_PHONE = makeOrder.ReceiverPhone,
                PRODUCT_TYPE = "HH",
                PRODUCT_WEIGHT = totalWeight,
               // MONEY_COLLECTION = (int)totalPrice,
                ORDER_PAYMENT = makeOrder.PaymentType,
                ORDER_SERVICE = makeOrder.OrderService,
                ORDER_NOTE = makeOrder.OrderNote,
                LIST_ITEM = listItem

            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var token = await _tokenHandler.GetTokenVTPAsync();
            string MakeOderUrl = _configuration["VTP:MakeOderUrl"];

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("token", token);

                var response = await client.PostAsync(MakeOderUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Request failed with status code: {response.StatusCode}");
                }
                var responseString = await response.Content.ReadAsStringAsync();
                JObject jsonResponse = JObject.Parse(responseString);
                var result = jsonResponse["data"]["ORDER_NUMBER"].ToString();
                var feeship = int.Parse(jsonResponse["data"]["MONEY_TOTAL_FEE"].ToString()) + int.Parse(jsonResponse["data"]["MONEY_VAT"].ToString());

                newOrder.TrackingNumber = result;
                newOrder.ShipPrice = feeship;
                await _unitOfWork.RepositoryOrderStatusLog.Insert(new OrderStatusLog
                {
                    LogAt = DateTime.Now,
                    OrderId = newOrder.Id,
                    StatusId = newOrder.StatusId,
                    TextLog = $"Order No.{newOrder.Id} Update at {DateTime.Now}"
                });
                newOrder.Price = totalPrice;
                await _unitOfWork.CommitAsync();

                if (makeOrder.PaymentType == 2)
                {
                    var shipcodresult = await _paymentService.ShipCOD(username, (int)newOrder.Id);
                    if (shipcodresult.Success)
                    {
               
                        newOrder.StatusId = 5;

                    }
                    else
                    {
                        throw new Exception(shipcodresult.message);
                    }
                }
                if (makeOrder.PaymentType == 1)
                {
                    var paymentOrder = await _paymentService.MakePayment(new PaymentInfoRequest { OrderId = (int)newOrder.Id, ip = makeOrder.UserIP }, username);
                    if (paymentOrder.Success)
                    {
                     
                        paymentUrl = paymentOrder.Value;
                        newOrder.StatusId = 2;
                    }
                    else
                    {
                        throw new Exception(paymentOrder.message);
                    }
                }



                _unitOfWork.RepositoryCart.Delete(listCartorder);
                await  _unitOfWork.CommitAsync();
                return new()
                {
                    Success = true,
                    Value = new MakeOrderReponseDTO
                    {
                        ORDERCODE = result,
                        PaymentURL = paymentUrl
                    }
                };
            }

        }
        
        public double PriceDiscount(double price, double discount)
        {
            return price;
        }

        public double DiscountVoucher(double discount, Voucher voucher) 
        { 
            return discount; 
        }


    }
}
