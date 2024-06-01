using Data.ViewModel.Helper;

namespace Service.Helper
{
    public interface IMediaHelper
    {
        Task<ReturnMediaModel> SaveMedia(IFormFile file);
        Task<List<ReturnMediaModel>> SaveMedias(List<IFormFile> files);
        string ValidateFile(IFormFile file);
    }
}