namespace Service.Helper
{
    public static class FileValidationHelper
    {
        private static readonly HashSet<string> AllowedImageMimeTypes = new HashSet<string>
    {
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/bmp"
    };

        private static readonly HashSet<string> AllowedVideoMimeTypes = new HashSet<string>
    {
        "video/mp4",
        "video/mpeg",
        "video/quicktime",
        "video/x-ms-wmv",
        "video/x-msvideo",
        "video/x-flv"
    };

        private static readonly HashSet<string> AllowedImageExtensions = new HashSet<string>
    {
        ".jpeg",
        ".jpg",
        ".png",
        ".gif",
        ".bmp"
    };

        private static readonly HashSet<string> AllowedVideoExtensions = new HashSet<string>
    {
        ".mp4",
        ".mpeg",
        ".mov",
        ".wmv",
        ".avi",
        ".flv"
    };

        public static bool IsValidImage(IFormFile file)
        {
            return file != null &&
                   AllowedImageMimeTypes.Contains(file.ContentType.ToLower()) &&
                   AllowedImageExtensions.Contains(Path.GetExtension(file.FileName).ToLower());
        }

        public static bool IsValidVideo(IFormFile file)
        {
            return file != null &&
                   AllowedVideoMimeTypes.Contains(file.ContentType.ToLower()) &&
                   AllowedVideoExtensions.Contains(Path.GetExtension(file.FileName).ToLower());
        }
    }
}
