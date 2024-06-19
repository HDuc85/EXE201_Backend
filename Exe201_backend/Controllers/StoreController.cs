using Data.Models;
using Data.ViewModel.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Service.Helper.Header;
using Service.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Exe201_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {

        private readonly IStoreService _StoreService; 


        public  StoreController(IStoreService storeService)
        {
            _StoreService = storeService;
        }


    

        /// <summary>
        /// Get All Store (Only role Admin)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _StoreService.GetAll();

            if(result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        /// <summary>
        /// Get Store with Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/<StoreController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Get(int id)
        {
           var resutl = await _StoreService.GetById(id);

            if(resutl == null)
            {
                return NotFound();
            }
            return Ok(resutl);
        }
        /// <summary>
        /// Get all member in store with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("members")]
        [Authorize(Roles = "admin,store admin,staff")]
        public async Task<IActionResult> GetMember(int id)
        {
            
            var username = User.GetUserName();
            var resutl = await _StoreService.GetAllMember(username ,id);

            if (resutl == null)
            {
                return NotFound();
            }
            return Ok(resutl);
        }
        /// <summary>
        /// Get all items in Store with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("items")]
        [Authorize(Roles = "admin,store admin,staff")]
        public async Task<IActionResult> Getitems(int id)
        {
            var username = User.GetUserName();
            var resutl = await _StoreService.GetAllItemStore(username, id);

            if (resutl == null)
            {
                return NotFound();
            }
            return Ok(resutl);
        }
        /// <summary>
        /// Create new Store
        /// </summary>
        /// <param name="value"></param>
        // POST api/<StoreController>
        [HttpPost]
        [Authorize(Roles = "admin,store admin")]

        public async Task<StoreVM> Post([FromForm] StoreCreateRequest request)
        {

            var username = User.GetUserName() ;
            return await _StoreService.CreateStore(username,request);
            
        }
        /// <summary>
        /// Insert member for store with username or email member
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("InsertMember")]
        [Authorize(Roles = "admin,store admin")]

        public async Task<IActionResult> InsetMember([FromBody] MemberRequest request)
        {
            var Username = User.GetUserName();
            var result =  await _StoreService.InsetMember(Username,request);
            if(result.Success)
            {
                return Ok(result);
            }
            return Ok(result);

        }
        /// <summary>
        /// Insert Item have exits in Store 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("InsertItem")]
        [Authorize(Roles = "admin,store admin,staff")]

        public async Task<IActionResult> InsertItem([FromBody]ItemRequest request)
        {
            var Username = User.GetUserName();
            var result = await _StoreService.InsertItem(Username, request);
            if (result.Success)
            {
                return Ok(result);
            }
            return Ok(result);

        }
        
        /// <summary>
        /// Update Store (Only Admin Store)
        /// </summary>
        /// <param name="request"></param>
        
        [HttpPut]
        [Authorize(Roles = "admin,store admin")]

        public async Task<IActionResult> Put(UpdateStoreRequest request)
        {
            var Username = User.GetUserName();
            var result = await _StoreService.UpdateStore(Username, request);
            return Ok(result);
        }
        /// <summary>
        /// Delete Store with id
        /// </summary>
        /// <param name="id"></param>
        // DELETE api/<StoreController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,store admin")]

        public async Task<IActionResult> Delete(int id)
        {
            var username = User.GetUserName();
            var result = await _StoreService.DeleteStore(username,id);
            return Ok(result);  
        }
        /// <summary>
        /// Delete Item of Store
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("StoreItem")]
        [Authorize(Roles = "admin,store admin,staff")]

        public async Task<IActionResult> DeleteStoreItem(ItemRequest request)
        {
            var username = User.GetUserName();
            var result = await _StoreService.DeleteStoreItem(username, request);
            return Ok(result);
        }
        /// <summary>
        /// Delete Member of Store
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("StoreMember")]
        [Authorize(Roles = "admin,store admin")]

        public async Task<IActionResult> DeleteStoreMember(MemberRequest request)
        {
            var username = User.GetUserName();
            var result = await _StoreService.DeleteStoreMember(username, request);
            return Ok(result);
        }
    }
}
