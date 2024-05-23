using Service.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;

using Service.Repo;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Service.Service.System.Authen
{
    public class AuthenService : IAuthenService
    {
        private PostgresContext _postgresContext;
        private readonly IConfiguration _config;
        public AuthenService(PostgresContext postgresContext, IConfiguration config) {

            _postgresContext = postgresContext;
            _config = config;
        }

       public async Task<string> Authencate(ViewModel.System.LoginRequest requset)
        {
            /* var user = await _postgresContext.Users.FindAsync(requset.Username);
             if (user == null) return null;
             var result = await _postgresContext.Users.FindAsync(requset.Password);
             if (!result.Succeeded) return null;
             var role = await _userManager.GetRolesAsync(user);
             var claims = new[]
             {
                 new Claim(ClaimTypes.Email,user.Email),
                 new Claim(ClaimTypes.GivenName, user.FirstName),
                 new Claim(ClaimTypes.Role, string.Join(";",role)),
             };

             var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
             var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

             var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                 _config["Tokens:Issuer"],
                 claims,
                 expires: DateTime.Now.AddHours(3),
                 signingCredentials: creds);
             return new JwtSecurityTokenHandler().WriteToken(token);*/
            return "fail";

        }

        public async Task<bool> Register(ViewModel.System.RegisterRequest request)
        {
           /* var user = await _userManager.FindByNameAsync(request.UserName);
            if (user != null)
            {
                return false;
            }

            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return false ;
            }
            user = new User()
            {
                Address = request.Address,
                Birthday = request.Birthday,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                UserName = request.UserName,
            };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded) return true;*/
            return false;
        }
    }
}
