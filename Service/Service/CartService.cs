using Data.Models;
using Microsoft.AspNetCore.Identity;
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


    }
}
