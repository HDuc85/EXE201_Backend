using Data.Models;
using Data.ViewModel.Product;
using Data.ViewModel.Tag;
using Data.ViewModel.Voucher;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Interface;
using System.Collections.Generic;

namespace Exe201_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly PostgresContext _context;
        private readonly ITagService _tagService;
        public TagController (PostgresContext context, ITagService tagService)
        {
            _context = context;
            _tagService = tagService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTags()
        {
            var tags = await _context.Tags.ToListAsync();
            if (tags == null)
            {
                return NotFound();
            }
            return Ok(tags);

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Tag>> GetTag(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
            return Ok(tag);
        }
        [HttpPost]
        public async Task<ActionResult<Tag>> CreateTag([FromForm]CreateTagDTO createTagDto)
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
        public async Task<ActionResult<Tag>> DeleteTag([FromForm]int id)
        {
            var product = await _tagService.DeleteTag( id);
            return Ok(product);
        }
    }
}
