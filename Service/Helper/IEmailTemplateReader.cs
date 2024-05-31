
namespace Service.Helper
{
    public interface IEmailTemplateReader
    {
        Task<string> GetTemplate(string templateName);
    }
}