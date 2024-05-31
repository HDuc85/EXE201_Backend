namespace Service.Helper
{
    public class EmailTemplateReader : IEmailTemplateReader
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmailTemplateReader(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// Get template email from rootpath
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        public async Task<string> GetTemplate(string templateName)
        {
            string templateEmail = Path.Combine(_webHostEnvironment.ContentRootPath, templateName);
            string content = await File.ReadAllTextAsync(templateEmail);

            return content;
        }
    }
}
