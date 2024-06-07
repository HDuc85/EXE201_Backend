using Data.ViewModel.Helper;

namespace Service.Helper.Media
{
    public interface IMediaHelper
    {
        Task<ReturnMediaModel> SaveMedia(IFormFile file, string path);
        Task<List<ReturnMediaModel>> SaveMedias(List<IFormFile> files, string path);
        string ValidateFile(IFormFile file);
    }
}