using Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace Service.Repo
{
    public interface ISizeService
    {
        Task<Size> GetSizeByNameAsync(string sizeName);
        Task AddSizeAsync(Size size);
    }
}
