using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data.Models;
using Service.Interface;
using Service.Repo;
using Data.ViewModel.Product;
using Data.ViewModel;
using static System.Net.Mime.MediaTypeNames;
using Data.ViewModel.User;
using Firebase.Auth;
using Service.Helper;
using Data.ViewModel.Voucher;

namespace Service.Service.System.Voucher
{
    public class VoucherService : IVoucherService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediaHelper _mediaHelper;

        public VoucherService(UnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IMediaHelper mediaHelper)
        {

            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mediaHelper = mediaHelper;
        }

        public async Task<Data.Models.Voucher> CreateVoucher(CreateVoucherDTO createVoucherDto)
        {

            var voucher = new Data.Models.Voucher
            {
                VoucherName = createVoucherDto.VoucherName,
                Type = createVoucherDto.Type,
                Value = createVoucherDto.Value,
                DateStart = createVoucherDto.DateStart,
                DateEnd = createVoucherDto.DateEnd

            };
            voucher.IsActive = true;
            _unitOfWork.RepositoryVoucher.Insert(voucher);
            await _unitOfWork.CommitAsync();

            return voucher;
        }

        public async Task<Data.Models.Voucher> UpdateVoucher(int voucherId, UpdateVoucherDTO updateVoucherDto)
        {
            var voucher = await _unitOfWork.RepositoryVoucher.GetById(voucherId);
            if (voucher == null)
            {
                throw new KeyNotFoundException("Voucher not found");
            }

            voucher.VoucherName = updateVoucherDto.VoucherName ?? voucher.VoucherName;
            voucher.Type = updateVoucherDto.Type ?? voucher.Type;
            voucher.Value = updateVoucherDto.Value ?? voucher.Value;
            voucher.DateStart = updateVoucherDto.DateStart ?? voucher.DateStart;
            voucher.DateEnd = updateVoucherDto.DateEnd ?? voucher.DateEnd;

            _unitOfWork.RepositoryVoucher.Update(voucher);

            // Save changes to the database
            try
            {
                await _unitOfWork.CommitAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await VoucherExists(voucherId))
                {
                    throw new KeyNotFoundException("Voucher not found during update");
                }
                else
                {
                    throw;
                }
            }

            return voucher;
        }
        private async Task<bool> VoucherExists(int voucherId)
        {
            var voucher = await _unitOfWork.RepositoryVoucher.GetById(voucherId);
            return voucher != null;
        }
    }
}
