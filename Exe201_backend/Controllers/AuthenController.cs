using Data.ViewModel.Authen;
using Data.ViewModel.System;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Linq;
using System.Threading.Tasks;

namespace Exe201_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenHandler _tokenHandler;
        private readonly IValidator<LoginRequest> _validator;

        public AuthenController(IUserService userService, ITokenHandler tokenHandler, IValidator<LoginRequest> validator)
        {
            _userService = userService;
            _tokenHandler = tokenHandler;
            _validator = validator;
        }

        /// <summary>
        /// Login with Username and password
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var validations = await _validator.ValidateAsync(loginRequest);
            if (!validations.IsValid)
            {
                return BadRequest(validations.Errors.Select(x => new ErrorValidation
                {
                    FieldName = x.PropertyName,
                    ErrorMessage = x.ErrorMessage,
                }));
            }

            var apiResult = await _userService.CheckLogin(loginRequest);

            if (!apiResult.Success)
            {
                return Unauthorized(apiResult.message);
            }

            (string accessToken, DateTime expiredDateAccessToken) = await _tokenHandler.CreateAccessToken(apiResult.Value);
            (string refreshToken, DateTime expiredDateRefreshToken, string codeRefreshToken) = await _tokenHandler.CreateRefreshToken(apiResult.Value);

            return Ok(new JwtModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Username = apiResult.Value.UserName,
                FirstName = apiResult.Value.Firstname,
                AccessTokenExpiredDate = expiredDateAccessToken
            });
        }

        /// <summary>
        /// Make new access token with refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
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
