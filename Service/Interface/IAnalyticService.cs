using Data.ViewModel;
using Data.ViewModel.Analytic;
using Data.ViewModel.Box;

namespace Service.Interface
{
    public interface IAnalyticService
    {
        Task<int> UserCount();
        Task<int> ProductCount();
        Task<int> OrderCount();
        Task<double> TotalPriceCount();
        Task<List<TopBuyProduct>> topBuyProduct();
        Task<List<TopUserBuy>> topUserBuy();
    }
}
