using Service.Interface;

namespace Service.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public OrderService(IUnitOfWork unitOfWork, IUserService userService) 
        {
            _unitOfWork = unitOfWork;   
            _userService = userService;
        }
        
     

    }
}
