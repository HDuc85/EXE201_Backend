using Data.DataViewModel.System;
using Data.Models;
using Service.Interface;

namespace Service.Service
{
    public class UserService : IUserService
    {
        IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User> CheckLogin(LoginRequest loginRequest)
        {
            return await _unitOfWork.RepositoryUser.GetSingleByCondition(u => u.UserName == loginRequest.Username && u.Password == loginRequest.Password);
        }

        public async Task<User> FindByUsername(string username)
        {
            return await _unitOfWork.RepositoryUser.GetSingleByCondition(u => u.UserName == username);
        }

        public async Task<User> FindById (Guid userId)
        {
            return await _unitOfWork.RepositoryUser.GetSingleByCondition(u => u.Id == userId);
        }
    }
}
