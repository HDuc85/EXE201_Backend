﻿using Data.Models;
using Data.ViewModel.System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Service.Interface;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Service.Service
{
    public class TokenHandler : ITokenHandler
    {
        IConfiguration _configuration;
        IUserService _userService;
        IUserTokenService _userTokenService;
        public TokenHandler(IConfiguration configuration, IUserService userService, IUserTokenService userTokenService)
        {
            _userService = userService;
            _configuration = configuration;
            _userTokenService = userTokenService;
        }
        public async Task<(string,DateTime)> CreateAccessToken(User user)
        {
            DateTime expiredToken = DateTime.Now.AddMinutes(30);
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString(),ClaimValueTypes.String,_configuration["JWT:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Iss, _configuration["JWT:Issuer"],ClaimValueTypes.String,_configuration["JWT:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.Now).ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Aud, _configuration["JWT:Audience"],ClaimValueTypes.String,_configuration["JWT:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Exp, DateTime.Now.AddMinutes(30).ToString("dd/MM/yyyy hh:mm:ss"),ClaimValueTypes.String,_configuration["JWT:Issuer"]),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(),ClaimValueTypes.String,_configuration["JWT:Issuer"]),
                new Claim(ClaimTypes.Name,user.FirstName,ClaimValueTypes.String,_configuration["JWT:Issuer"]),
                new Claim("Username", user.UserName, ClaimValueTypes.String, _configuration["JWT:Issuer"])
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenInfo = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credential
                );

            string accessToken = new JwtSecurityTokenHandler().WriteToken(tokenInfo);

            return await Task.FromResult((accessToken, expiredToken));
        }

        public async Task<(string, DateTime,string)> CreateRefreshToken(User user)
        {
            DateTime expiredToken = DateTime.Now.AddHours(3);
            string code = Guid.NewGuid().ToString();
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString(),ClaimValueTypes.String,_configuration["JWT:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Iss, _configuration["JWT:Issuer"],ClaimValueTypes.String,_configuration["JWT:Issuer"]),
                 new Claim(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.Now).ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Aud, _configuration["JWT:Audience"],ClaimValueTypes.String,_configuration["JWT:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Exp, DateTime.Now.AddHours(3).ToString("dd/MM/yyyy hh:mm:ss"),ClaimValueTypes.String,_configuration["JWT:Issuer"]),
               new Claim(ClaimTypes.SerialNumber,code,ClaimValueTypes.String, _configuration["JWT:Issuer"]),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenInfo = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credential
                );

            string refreshToken = new JwtSecurityTokenHandler().WriteToken(tokenInfo);

            return await Task.FromResult((refreshToken, expiredToken, code));
        }

        public async Task ValidateToken(TokenValidatedContext context)
        {
            var claims = context.Principal.Claims.ToList();

            if(claims.Count == 0)
            {
                 context.Fail("Token has no info");
                return;
            }

            var identity = context.Principal.Identity as ClaimsIdentity;

            if(identity.FindFirst(JwtRegisteredClaimNames.Iss) == null)
            {
                context.Fail("Token is not issued by point entry");
                return;
            }

            if(identity.FindFirst("Username") == null)
            {
                string username = identity.FindFirst("Username").Value;
                
                var user = await _userService.FindByUsername(username);
                if(user == null) {
                    context.Fail("Token is invalid for user");
                    return;
                
                }

            }

            if(identity.FindFirst(JwtRegisteredClaimNames.Exp) == null) {
                var dateExp = identity.FindFirst(JwtRegisteredClaimNames.Exp).Value;

                long exp = long.Parse(dateExp);
                var date = DateTimeOffset.FromUnixTimeSeconds(exp).DateTime;

                var minutes = date.Subtract(DateTime.Now).TotalMinutes;

                if(minutes < 0) {
                    context.Fail("Token is expired");

                    throw new Exception("Token is expired");
                    

                }

            }


        }

        public async Task<JwtModel> ValidateRefreshToken(string refreshToken)
        {
            JwtModel jwtModel = new ();

           

            var claimPriciple = new JwtSecurityTokenHandler().ValidateToken(refreshToken, new TokenValidationParameters
            {
                RequireExpirationTime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"])),
                ValidateIssuerSigningKey = true,
                
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidIssuer = _configuration["JWT:Issuer"],
                ValidAudience = _configuration["JWT:Audience"],
                ClockSkew = TimeSpan.Zero,
            }, out _
            );

            if (claimPriciple == null) return new();

            string code = claimPriciple.Claims.FirstOrDefault(s => s.Type == ClaimTypes.SerialNumber)?.Value;

            if(string.IsNullOrEmpty(code)) return new();

            UserToken userToken = await _userTokenService.CheckRefreshToken(code);

            if(userToken != null)
            {
                User user = await _userService.FindById(userToken.UserId);
                (string newAccessToken, DateTime createdDateAccessToken) = await CreateAccessToken(user);
                (string newRefreshToken, DateTime createdDateRefreshToken, string newCode) = await CreateRefreshToken(user);

                return new JwtModel
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    Username = user.UserName,
                    FirstName = user.FirstName
                };
            }
            return new();

        }

    }
}