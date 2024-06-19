using Data.Models;
using Data.ViewModel.Product;
using Data.ViewModel.Voucher;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Repo;
using Service.Service.System.Product;

namespace Exe201_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : Controller
    {
        private readonly PostgresContext _context;
        private readonly IVoucherService _voucherService;
        public VoucherController(PostgresContext context, IVoucherService voucherService)
        {
            _context = context;
            _voucherService = voucherService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Voucher>>> GetVouchers()
        {
            var vouchers = await _context.Vouchers.ToListAsync();
            if (vouchers == null)
            {
                return NotFound();
            }
            return Ok(vouchers);

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Voucher>> GetVoucher(int id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null)
            {
                return NotFound();
            }
            return Ok(voucher);
        }
        [HttpPost]
        public async Task<ActionResult<Voucher>> CreateVoucher(CreateVoucherDTO createVoucherDto)
        {
            var voucher = await _voucherService.CreateVoucher(createVoucherDto);
            return Ok(voucher);
            // Kiểm tra tính hợp lệ của dữ liệu
           
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVoucher(int id, UpdateVoucherDTO updateVoucherDto)
        {
            var voucher = await _voucherService.UpdateVoucher(id, updateVoucherDto);
            return Ok(voucher);
           
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVoucher(int id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null)
            {
                return NotFound();
            }

            voucher.IsActive = false;
            _context.Entry(voucher).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }



    }
}
