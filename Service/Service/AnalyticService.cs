using Microsoft.AspNetCore.Identity;
using Service.Interface;
using Data.Models;
using Data.ViewModel;
using Data.ViewModel.Box;
using Microsoft.AspNetCore.Identity;
using Service.Interface;
using Data.ViewModel.Analytic;
using Microsoft.EntityFrameworkCore;

namespace Service.Service
{
    public class AnalyticService : IAnalyticService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public AnalyticService(UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public Task<int> OrderCount()
        {
            var count = _unitOfWork.RepositoryOrder.Table.Count();
            return Task.FromResult(count);
        }

        public Task<int> ProductCount()
        {
            var count = _unitOfWork.RepositoryProduct.Table.Count();
            return Task.FromResult(count);
        }

        public async Task<List<TopBuyProduct>> topBuyProduct()
        {
            var summaries = await _unitOfWork.RepositoryOrderItem.Table
            .GroupBy(o => new { o.ProductVariantId, o.BoxId })
            .Select(g => new OrderItemSummary
            {
                ProductVariantId = g.Key.ProductVariantId,
                BoxId = g.Key.BoxId,
                TotalQuantity = g.Sum(o => (int)o.Quantity),
                TotalPrice = g.Sum(o => (double)o.Price * (int)o.Quantity)
            })
            .ToListAsync();

            var ListBox = await _unitOfWork.RepositoryBox.GetAll();
            var ListProduct = await _unitOfWork.RepositoryProduct.GetAll();


            var result = summaries.Select(x => new TopBuyProduct
            {
                ProductName = x.ProductVariantId.HasValue?
                                ListProduct.FirstOrDefault(y => y.Id == x.ProductVariantId).ProductName
                                : ListBox.FirstOrDefault(y => y.Id == x.BoxId).BoxName,
                TotalSelled = x.TotalQuantity
            }).ToList();

            return result;
        }

        public async Task<List<TopUserBuy>> topUserBuy()
        {

            var summaries = await _unitOfWork.RepositoryOrder.Table
            .GroupBy(o => new { o.UserId })
            .Select(g => new UserBuySummary
            {
                id = g.Key.UserId,
                price = g.Sum(o => (double)o.Price),
            })
            .ToListAsync();

            var listUser = await _unitOfWork.RepositoryUser.GetAll();

            var result = summaries.Select(x => new TopUserBuy
            {
                Name = listUser.FirstOrDefault(y => y.Id.Equals(x.id)).Firstname,
                TotalBuy = x.price

            }).ToList();
            return result;
        }

        public async Task<double> TotalPriceCount()
        {
            var total =  _unitOfWork.RepositoryOrder.Table.Sum(x => x.Price);
            return (double)total;
        }

        public Task<int> UserCount()
        {
            var count = _unitOfWork.RepositoryUser.Table.Count();
            return Task.FromResult(count);
        }
    }
}
