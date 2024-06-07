using Data.Models;
using Data.ViewModel.Helper;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IO;

namespace Service.Helper.Media
{
    public class MediaHelper : IMediaHelper
    {
        FirebaseConfigModel _firebaseConfig;
        public MediaHelper(IOptions<FirebaseConfigModel> options)
        {

            _firebaseConfig = options.Value;

        }

        public async Task<ReturnMediaModel> SaveMedia(IFormFile file, string path)
        {
            var type = ValidateFile(file);
            if (type.IsNullOrEmpty())
            {
                throw new ArgumentException("File is not image or video");
            }
            var stream = file.OpenReadStream();
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var storagePath = $"{fileName}";
            var auth = new FirebaseAuthProvider(new FirebaseConfig(_firebaseConfig.ApiKey));
            var signin = await auth.SignInWithEmailAndPasswordAsync(_firebaseConfig.Email, _firebaseConfig.Password);
            var result = await new FirebaseStorage(
                 _firebaseConfig.Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(signin.FirebaseToken),
                        ThrowOnCancel = true,
                    })
                    .Child(path)
                    .Child(type)
                    .Child(storagePath)
                    .PutAsync(stream);
            return new ReturnMediaModel
            {
                type = type,
                url = result
            };

        }
        public async Task<List<ReturnMediaModel>> SaveMedias(List<IFormFile> files, string path)
        {
            List<ReturnMediaModel> result = new List<ReturnMediaModel>();
            foreach (var file in files)
            {
                result.Add(await SaveMedia(file, path));
            }
            return result;
        }


        public string ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is not selected or empty.");
            }



            if (FileValidationHelper.IsValidVideo(file))
            {

                if (file.Length > _firebaseConfig.LimitSize * 1024 * 1024)
                {
                    throw new Exception($"Video size allowed only below {_firebaseConfig.LimitSize}.");
                }
                return "videos";
            }
            if (FileValidationHelper.IsValidImage(file))
            {
                return "images";
            }
            return string.Empty;
        }
    }
}
