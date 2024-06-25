using Data.Models;
using Data.ViewModel.Product;
using Data.ViewModel.Tag;
using Data.ViewModel.Voucher;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Interface;
using Service.Service;
using System.Collections.Generic;

namespace Exe201_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ITagService _tagService;
        public TagController(ITagService tagService, UnitOfWork unitOfWork)
        {
            _tagService = tagService;
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTags()
        {
            var query = _unitOfWork.RepositoryTag.GetAllWithCondition()
                                                     .Include(x => x.TagValues);

            var tags = await query.ToListAsync();
            return tags;

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Tag>> GetTag(int id)
        {
            var tag = await _unitOfWork.RepositoryTag.GetSingleByCondition(p => p.Id == id);
            if (tag == null)
            {
                return NotFound();
            }
            return Ok(tag);
        }
        [HttpPost]
        public async Task<ActionResult<Tag>> CreateTag([FromForm] CreateTagDTO createTagDto)
        {
            var product = await _tagService.CreateTag(createTagDto);
            return Ok(product);
        }
        [HttpPut]
        public async Task<ActionResult<Tag>> UpdateTag([FromForm] UpdateTagDTO updateTagDTO, int id)
        {
            var product = await _tagService.UpdateTag(updateTagDTO, id);
            return Ok(product);
        }
        [HttpDelete]
        public async Task<ActionResult<Tag>> DeleteTag([FromForm] int id)
        {
            var product = await _tagService.DeleteTag(id);
            return Ok(product);
        }
    }
}
