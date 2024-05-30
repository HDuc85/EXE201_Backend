using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Data.Models;
using Service.Interface;
using Service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Data.ViewModel.User;
using Microsoft.AspNetCore.Http.HttpResults;
using Service.Helper;
using Data.ViewModel.Helper;
using System.ComponentModel.Design;

namespace Exe201_backend.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    
    public class UsersController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        private IUserService _userService;
        private IEmailHelper _emailHelper;
       
        public UsersController(IUnitOfWork unitOfWork,IUserService userService, IEmailHelper emailHelper )
        {
            _emailHelper = emailHelper;
            _userService = userService;
            _unitOfWork = unitOfWork;
           
        }

        // GET: api/Users
        /// <summary>
        /// Only role Admin
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin")]

        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {

            var test = await _unitOfWork.RepositoryUser.GetAll();
           
            return Ok(test);
        }

        // GET: api/Users/5
        /// <summary>
        /// Only role Admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize (Roles = "admin")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            var user = await _unitOfWork.RepositoryUser.GetById(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> PutUser([FromBody]UpdateUserRequest updateUserRequest)
        {

          var  results =  await _userService.UpdateUser(updateUserRequest);
            if (!results.Success)
            {
                BadRequest(results.message);
            }
            return Ok(results.message);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromForm]RegisterRequest register)
        {

            var (result,token) = await _userService.Register(register);


            if (result.Success)
            {
              
                string url = Url.Action("ComfirmEmail", "Users", new {userId = result.Value.Id, token},Request.Scheme);
            
                if (!await _userService.SendEmailComfirm(url, result.Value))
                    return BadRequest();
                return Ok(result.message);
            }
            
            return BadRequest(result.message);
        }

        [HttpGet ("ComfirmEmail")]
   
        public async Task<IActionResult> ComfirmEmail(string userId, string token)
        {
            var result = await _userService.ComfirmEmail(userId,token);

            if (!result.Success)
            {
                return BadRequest(result.message);
            }
            return Ok(result.message);
        }

        [HttpGet("ForgetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPassword(string email,string host)
        {
            var result = await _userService.ForgetPassword(email,host);

            if(result.Success)
            {  return Ok(result.message); }
            return BadRequest(result.message);
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordRequest resetPasswordRequest)
        {
            var resutlt = await _userService.ResetPassword(resetPasswordRequest);

            if(resutlt.Success)
            {
                return Ok(resutlt.message);
            }

            return BadRequest(resutlt.message);

        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize (Roles = "admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _unitOfWork.RepositoryUser.GetById(id);
            if (user == null)
            {
                return NotFound();
            }

            _unitOfWork.RepositoryUser.Delete(user);
            await _unitOfWork.CommitAsync();

            return NoContent();
        }

        
    }
}
