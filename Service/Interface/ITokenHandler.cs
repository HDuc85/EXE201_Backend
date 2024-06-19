using Data.Models;
using Data.ViewModel.System;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Service.Interface
{
    public interface ITokenHandler
    {
        Task<(string, DateTime, string)> CreateRefreshToken(User user);
        Task<(string, DateTime)> CreateAccessToken(User user);
        Task ValidateToken(TokenValidatedContext context);
        Task<JwtModel> ValidateRefreshToken(string refreshToken);
        Task<string> GetTokenVTPAsync();
    }
}