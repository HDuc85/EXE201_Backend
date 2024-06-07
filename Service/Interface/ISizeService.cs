using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Service.ViewModel.System;

namespace Service.Repo
{
    public interface ISizeService
    {
        Task<Size> GetSizeByNameAsync(string sizeName);
        Task AddSizeAsync(Size size);
    }
}
