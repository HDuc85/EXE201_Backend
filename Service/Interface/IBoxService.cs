using Data.ViewModel;
using Data.ViewModel.Box;

namespace Service.Interface
{
    public interface IBoxService
    {
        Task<ApiResult<bool>> CreateBox(CreateBoxRequest request);
        Task<ApiResult<BoxViewModel>> GetBoxById(int Id);
        Task<ApiResult<List<BoxViewModel>>> GetAllBoxes();
        Task<ApiResult<bool>> UpdateBox(UpdateBoxRequest request);
        Task<ApiResult<bool>> DeleteBox(int Id);
    }
}
