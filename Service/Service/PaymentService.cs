using Data.Models;
using Data.ViewModel;
using Data.ViewModel.Payment;
using Service.Helper.VNPay;
using Service.Interface;
using Service.Service;

namespace Service.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;


        public PaymentService(IUnitOfWork unitOfWork, IConfiguration configuration, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _configuration = configuration;
        }

        public async Task<ApiResult<bool>> UpdatePaymentStatus(int paymentId, int paymentStatus, int StatusOrderId)
        {
            var paymentdetail = await _unitOfWork.RepositoryPaymentDetail.GetById(paymentId);
            if (paymentdetail == null)
            {
                return new()
                {
                    Success = false,
                    message = "Payment is not exits"
                };
            }
            var order = await _unitOfWork.RepositoryOrder.GetSingleByCondition(x => x.PaymentId == paymentId);
            var orderlog =  _unitOfWork.RepositoryOrderStatusLog.Insert(new OrderStatusLog
            {
                LogAt = DateTime.Now,
                OrderId = order.Id,
                StatusId = StatusOrderId,
                TextLog = $"Order No.{order.Id} Update Status PaymentStatus at {DateTime.Now}"
            });
            paymentdetail.PaymentStatusId = paymentStatus;
            await _unitOfWork.CommitAsync();
            return new()
            {
                Success = true,
                message = $"orderId {order.Id} is updated status payment"
            };
        }

        public async Task<IEnumerable<PaymentDetail>> GetAll()
        {
            return await _unitOfWork.RepositoryPaymentDetail.GetAll();
        }
        public async Task<ApiResult<string>> MakePayment(PaymentInfoRequest paymentInfoRequest, string username)
        {
            var user = await _userService.FindByUsername(username);
            if(user == null)
            {
                return new ApiResult<string>
                {
                    Success = false,
                    message = "User is not exist"
                };
            }
            var orderdetail = await _unitOfWork.RepositoryOrder.GetById(paymentInfoRequest.OrderId);
            if (orderdetail == null)
            {
                return new ApiResult<string>
                {
                    Success = false,
                    message = "Order invalid"
                };
            }

            if(orderdetail.UserId != user.Id)
            {
                return new ApiResult<string>()
                {
                    Success = false,
                    message = "You can make payment with order"
                };
            }

            if (orderdetail.PaymentId == null)
            {
                var paymentDetail = new PaymentDetail
                {

                    PaymentStatusId = 0,

                };
                await _unitOfWork.RepositoryPaymentDetail.Insert(paymentDetail);
                await _unitOfWork.RepositoryPaymentDetail.Commit();
               

                orderdetail.PaymentId = paymentDetail.Id;
                await _unitOfWork.CommitAsync();
            }

            var payment = await _unitOfWork.RepositoryPaymentDetail.GetById(orderdetail.PaymentId);

            payment.Amount = Convert.ToInt64(orderdetail.Price + orderdetail.ShipPrice);
            payment.CreatedDate = DateTime.Now;
            payment.Description = "payment order no." + orderdetail.Id;

            _unitOfWork.RepositoryPaymentDetail.Update(payment);


            string vnp_Returnurl = _configuration["VNPay:vnp_Returnurl"];
            string vnp_Url = _configuration["VNPay:vnp_Url"];
            string vnp_TmnCode = _configuration["VNPay:vnp_TmnCode"];
            string vnp_HashSecret = _configuration["VNPay:vnp_HashSecret"];

            if (string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret))
            {
                throw new Exception("Server chưa cấu hình tham số cho VNPay ");
            }

            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (payment.Amount * 100).ToString());


            vnpay.AddRequestData("vnp_CreateDate", payment.CreatedDate?.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", paymentInfoRequest.ip.ToString());


            vnpay.AddRequestData("vnp_Locale", "vn");

            vnpay.AddRequestData("vnp_OrderInfo", "payment order no." + paymentInfoRequest.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", paymentInfoRequest.OrderId.ToString());

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            var orderlog = _unitOfWork.RepositoryOrderStatusLog.Insert(new OrderStatusLog
            {
                LogAt = DateTime.Now,
                OrderId = paymentInfoRequest.OrderId,
                StatusId = 2,
                TextLog = $"Order No.{orderdetail.Id} is make payment with VNPAY at {DateTime.Now}"
            });
            await _unitOfWork.CommitAsync();

            return new ApiResult<string> { Success = true, Value = paymentUrl };
        }

        public async Task<ApiResult<PaymentDetailViewModel>> ShipCOD(string username, int OrderID)
        {
            var user = await _userService.FindByUsername(username);
            if(user == null)
            {
                return new ApiResult<PaymentDetailViewModel>()
                {
                    Success = false,
                    message = "User is not exist"
                };
            }

            var order = await _unitOfWork.RepositoryOrder.GetById(OrderID);

            if(order.UserId != user.Id)
            {
                return new ApiResult<PaymentDetailViewModel>()
                {
                    message = "You can make payment with order",
                    Success = false
                };
            }


            if(order == null)
            {
                return new ApiResult<PaymentDetailViewModel>() { Success = false, message = "Order is not exist" };
            }

            if(order.PaymentId == null)
            {
                PaymentDetail newPayment = new PaymentDetail()
                {
                    PaymentStatusId = 3,
                    Amount = Convert.ToInt64(order.Price + order.ShipPrice),
                    CreatedDate = DateTime.Now,
                    Description = "payment order no." + order.Id,
                };
                await _unitOfWork.RepositoryPaymentDetail.Insert(newPayment);
                await _unitOfWork.CommitAsync();
                order.PaymentId = newPayment.Id;
            }
            
            var Paymentdetail = await _unitOfWork.RepositoryPaymentDetail.GetById(order.PaymentId);

            if(Paymentdetail.PaymentStatusId == 2)
            {
                return new ApiResult<PaymentDetailViewModel>
                {
                    message = "Order have finished Payment",
                    Success = false
                };
            }

            if(Paymentdetail.PaymentStatusId != 3)
            {
                Paymentdetail.PaymentStatusId = 3;
            }
            order.StatusId = 5;
            var orderlog = _unitOfWork.RepositoryOrderStatusLog.Insert(new OrderStatusLog
            {
                LogAt = DateTime.Now,
                OrderId = order.Id,
                StatusId = 5,
                TextLog = $"Order No.{order.Id} make COD payment at {DateTime.Now}"
            });
            await _unitOfWork.CommitAsync();
            return new ApiResult<PaymentDetailViewModel>
            {
                Success = true,
                Value = new PaymentDetailViewModel()
                {
                    Amount = Paymentdetail.Amount,
                    CreateDate = Paymentdetail.CreatedDate,
                    Desc = Paymentdetail.Description,
                    OrderId = OrderID,
                    PaymentStatus = "COD payment"
                }
            };

        }
        public async Task<ApiResult<bool>> ValidPayment(IQueryCollection querylists) { 
            

        string vnp_HashSecret = _configuration["VNPay:vnp_HashSecret"];

        VnPayLibrary vnpay = new VnPayLibrary();
        var securehash = querylists.FirstOrDefault(x => x.Key == "vnp_SecureHash");
            if(vnpay.ValidateSignature(securehash.Value, vnp_HashSecret))
            {
                return new ApiResult<bool> { Success = false, message = "Invaild Payment request" };
            }

        PaymentDetail paymentDetail = new PaymentDetail();
        string vnp_ResponseCode = string.Empty;
            foreach (var key in querylists)
            {
                switch (key.Key)
                {
                    case "vnp_Amount":
                        paymentDetail.Amount = Convert.ToInt64(key.Value) / 100;
                        break;
                    case "vnp_TransactionNo":
                        paymentDetail.TransactionNo = Convert.ToInt64(key.Value);
                        break;
                    case "vnp_TxnRef":
                        paymentDetail.Id = Convert.ToInt32(key.Value);
                        break;
                    case "vnp_ResponseCode":
                        vnp_ResponseCode = key.Value;
                        break;
                    case "vnp_BankCode":
                        paymentDetail.BankCode = key.Value;
                        break;
                    case "vnp_BankTranNo":
                        paymentDetail.BankTranNo = key.Value;
                        break;
                    case "vnp_PayDate":
                        paymentDetail.PayDate = DateTime.ParseExact(key.Value, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                        break;
                }
            }
            var order = await _unitOfWork.RepositoryOrder.GetById(paymentDetail.Id);
            if (order == null)
                {
                return new ApiResult<bool>
                {
                    message = "Invalid Order",
                    Success = false,
                };
                }
            var update = await _unitOfWork.RepositoryPaymentDetail.GetSingleByCondition(i => i.Id == order.PaymentId);
            if (vnp_ResponseCode == "00")
            {
                update.PaymentStatusId = 2;
                update.BankTranNo = paymentDetail.BankTranNo;
                update.PayDate = paymentDetail.PayDate;
                update.Amount = paymentDetail.Amount;
                update.TransactionNo = paymentDetail.TransactionNo;
                update.BankCode = paymentDetail.BankCode;
                order.StatusId = 4;
                _unitOfWork.RepositoryPaymentDetail.Update(update);
                try
                {
                    var orderlog = _unitOfWork.RepositoryOrderStatusLog.Insert(new OrderStatusLog
                    {
                        LogAt = DateTime.Now,
                        OrderId = order.Id,
                        StatusId = 4,
                        TextLog = $"Order No.{order.Id} pay successfully at {DateTime.Now}"
                    });
                    await _unitOfWork.CommitAsync();
                    return new ApiResult<bool> { Success = true, message = "Thanh toán thành công" };

                }
                catch
                {
                    throw new Exception("Lỗi khi cập nhật database");
                }
            }
            else
            {
                update.PaymentStatusId = 1;
                try
                {
                    order.StatusId = 3;
                    var orderlog = _unitOfWork.RepositoryOrderStatusLog.Insert(new OrderStatusLog
                    {
                        LogAt = DateTime.Now,
                        OrderId = order.Id,
                        StatusId = 3,
                        TextLog = $"Order No.{order.Id} pay fail at {DateTime.Now}"
                    });
                    await _unitOfWork.CommitAsync();
                    return new ApiResult<bool> { Success = false, message = "Thanh toán thất bại" };


                }
                catch
                {
                    throw new Exception("Lỗi khi cập nhật database");
                }

            }
        }
    
        public async Task<ApiResult<PaymentDetailViewModel>> GetPaymentDetail(string username, int OrderId)
        {
            var user = await _userService.FindByUsername(username);

            if(user == null)
            {
                return new ApiResult<PaymentDetailViewModel>
                {
                    Success = false,
                    message = "User is not exits"
                };
            }

            var order = await _unitOfWork.RepositoryOrder.GetById(OrderId);
            if(order == null)
            {
                return new ApiResult<PaymentDetailViewModel>
                {
                    Success = false,
                    message = "Order is not exits"
                };
            }

            if(order.UserId != user.Id)
            {
                return new ApiResult<PaymentDetailViewModel>
                {
                    Success = false,
                    message = "You can not see this Order"
                };
            }

            if (!order.PaymentId.HasValue)
            {
                return new ApiResult<PaymentDetailViewModel>
                {
                    Success = false,
                    message = "Order have not already make Payment"
                };
            }

            var paymentdetail = await _unitOfWork.RepositoryPaymentDetail.GetById(order.PaymentId);

            if(paymentdetail == null)
            {
                return new ApiResult<PaymentDetailViewModel>
                {
                    Success = false,
                    message = "Payment Id in Order is not exits"
                };
            }

            var result = new PaymentDetailViewModel
            {
                Amount = paymentdetail.Amount,
                CreateDate = paymentdetail.CreatedDate,
                Desc = paymentdetail.Description,
                OrderId = (int)order.Id,
                PayDate = paymentdetail.PayDate,
                
            };

            switch (paymentdetail.PaymentStatusId)
            {
                case 0:
                    result.PaymentStatus = "Unpaid";
                    break;
                case 1:
                    result.PaymentStatus = "Payment failed";
                    break;
                case 2:
                    result.PaymentStatus = "Payment success";
                    break;
                case 3:
                    result.PaymentStatus = "COD payment";
                    break;
            }

            return new ApiResult<PaymentDetailViewModel>
            {
                Success = true,
                Value = result

            };

        }
    }
}
