﻿using Data.Models;
using Data.ViewModel;
using Data.ViewModel.Authen;
using Data.ViewModel.System;
using Data.ViewModel.User;
using System.Collections;

namespace Service.Interface
{
    public interface IUserService 
    {
        Task<ApiResult<string>> BanUser(string username, TimeAddInput time);
        Task<ApiResult<User>> CheckLogin(LoginRequest loginRequest);
        Task<bool> CheckUserBan(Guid userId);
        Task<ApiResult<bool>> ComfirmEmail(string userId, string tokenConfirm);
       
        Task<User> FindByEmail(string Email);
        Task<User> FindById(Guid userId);
        Task<User> FindByUsername(string username);
        Task<ApiResult<bool>> ForgetPassword(string email, string host);
        Task<IEnumerable<UserVM>> GetAll();
        Task<IEnumerable<User>> GetPageSize(int pageIndex, int pageSize);
        Task<(ApiResult<User>, string)> Register(RegisterRequest registerRequest);
        Task RemoveRole(Guid id, string[] roles);
        Task<ApiResult<bool>> ResetPassword(ResetPasswordRequest resetPasswordRequest);
        Task<IEnumerable<User>> Search(string key, int pageIndex, int pageSize);
        Task<bool> SendEmailComfirm(string Url, User user);
        Task<ApiResult<bool>> UpdateRole(UpdateRoleRequest updateRoleRequest);
        Task<ApiResult<User>> UpdateUser(UpdateUserRequest updateUserRequest);
        Task<User> UserExits(string Username);
        Task<ApiResult<bool>> DeleteUser(Guid userId);
       
    }
}