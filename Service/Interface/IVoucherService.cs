using Data.Models;
using Data.ViewModel;
using Data.ViewModel.Product;
using Data.ViewModel.Voucher;
using Microsoft.AspNetCore.Mvc;

namespace Service.Repo
{
    public interface IVoucherService
    {
        Task<Voucher> CreateVoucher(CreateVoucherDTO createVoucherDto);
        Task<Voucher> UpdateVoucher(int voucherId, UpdateVoucherDTO updateVoucherDto);

    }
}
