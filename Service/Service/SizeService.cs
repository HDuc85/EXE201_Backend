using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Service.ViewModel.System;
using Microsoft.EntityFrameworkCore;
using Data.Models;
using Service.Interface;
using System.Drawing;
using Service.Repo;

namespace Service.Service.System.Product
{
    public class SizeService : ISizeService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SizeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddSizeAsync(Data.Models.Size size)
        {
            await _unitOfWork.RepositorySize.Insert(size);
        }

        public async Task<Data.Models.Size> GetSizeByNameAsync(string sizeName)
        {
            return await _unitOfWork.RepositorySize.GetSingleByCondition(c => c.SizeValue == sizeName);
        }



    }
}
