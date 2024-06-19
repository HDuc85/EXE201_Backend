using Data.ViewModel;
using Data.ViewModel.Box;
using Data.ViewModel.Tag;

namespace Service.Interface
{
    public interface ITagService
    {
        Task<ApiResult<bool>> CreateTag(CreateTagDTO createTagDTO);
        Task<ApiResult<bool>> UpdateTag(UpdateTagDTO updateTagDTO, int id);
        Task<ApiResult<bool>> DeleteTag(int Id);
    }
}
