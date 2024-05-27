using Data.DataViewModel.System;
using Data.ViewModel.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;


namespace Exe201_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenController : ControllerBase
    {

        IUserService _userService;
        ITokenHandler _tokenHandler;
        IUserTokenService _userTokenService;
        public AuthenController(IUserService userService, ITokenHandler tokenHandler, IUserTokenService userTokenService)
        {
            _userService = userService;
            _tokenHandler = tokenHandler;
            _userTokenService = userTokenService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {

            if(loginRequest == null)
            {
                return BadRequest("Is not a user");
            }

            var user = await _userService.CheckLogin(loginRequest);

            if(user == null)
            {
                return Unauthorized("User or password is incorrect");
            }

            (string accessToken, DateTime expiredDateAccessToken) = await _tokenHandler.CreateAccessToken(user);
            (string refreshToken, DateTime expiredDateRefreshToken, string codeRefreshToken) = await _tokenHandler.CreateRefreshToken(user);

            await _userTokenService.SaveToken(new Data.Models.UserToken
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpireadDateAccessToken = expiredDateAccessToken,
                ExpireadDateRefreshToken = expiredDateRefreshToken,
                CodeRefreshToken = codeRefreshToken,
                UserId = user.Id,
                CreateDate = DateTime.Now,
                IsActive = true
            });

            return Ok(new JwtModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Username = user.UserName,
                FirstName = user.FirstName
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel refreshToken)
        {
            var validate = await _tokenHandler.ValidateRefreshToken(refreshToken.RefreshToken);
            if (validate.Username == null)
                return Unauthorized("Invalid RefreshToken");
            return Ok(validate);
        }
    }
}
