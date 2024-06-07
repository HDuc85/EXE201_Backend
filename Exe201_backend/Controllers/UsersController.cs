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
using System.Diagnostics.CodeAnalysis;
using Data.ViewModel.System;
using Microsoft.OpenApi.Expressions;

namespace Exe201_backend.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class UsersController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        private IUserService _userService;
        

        public UsersController(IUnitOfWork unitOfWork, IUserService userService)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;

        }

        // GET: api/Users
        /// <summary>
        /// Get all user and Only role Admin
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin")]

        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
           
            var users = await _unitOfWork.RepositoryUser.GetAll();

            if (users == null)
            {
                return NotFound();
            }
            return Ok(users);
        }

        // GET: api/Users/5
        /// <summary>
        /// Only role Admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            var user = await _unitOfWork.RepositoryUser.GetById(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
        /// <summary>
        /// Get list User with pagesize
        /// </summary>
        /// <param name="pageSize"></param>

        /// <returns></returns>
        [HttpGet("PageSize")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetPageSize(int pageIndex, int pageSize)
        {
            var users = await _unitOfWork.RepositoryUser.GetPageSize(pageIndex: pageIndex, pageSize: pageSize);
            if (users == null)
            {
                return NotFound();
            }
            return Ok(users);
        }

       

        [HttpGet("Search")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<User>>> Search(string key, int pageIndex, int pageSize)
        {

            var users = await _unitOfWork.RepositoryUser.GetPageSize(x => 
            x.UserName.Contains(key) || 
            x.Firstname.Contains(key) || 
            x.Lastname.Contains(key) || 
            x.Email.Contains(key) ||
            x.Address.Contains(key),
            pageIndex: pageIndex, 
            pageSize: pageSize);
            if (users == null)
            {
                return NotFound();
            }
            return Ok(users);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> PutUser([FromForm]UpdateUserRequest updateUserRequest)
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
        [AllowAnonymous]

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
        /// <summary>
        /// Update role with username and List Roles
        /// </summary>
        /// <param name="updateRoleRequest"></param>
        /// <returns></returns>
        [HttpPost ("UpdateRole")]
        [Authorize(Roles = "admin")]

        public async Task<IActionResult> UpdateUserRole([FromBody]UpdateRoleRequest updateRoleRequest)
        {
            var result = await _userService.UpdateRole(updateRoleRequest);

            if(!result.Success)
            {
                return BadRequest(result.message);
            }
            return Ok(result.message);
        }
        
    }
}
