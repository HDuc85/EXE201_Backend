using Data.DataViewModel.System;
using Data.Models;

namespace Service.Interface
{
    public interface IUserService
    {
        Task<User> CheckLogin(LoginRequest loginRequest);
        Task<User> FindById(Guid userId);
        Task<User> FindByUsername(string username);
    }
}