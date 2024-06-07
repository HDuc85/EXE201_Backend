namespace Service.Helper.Email
{
    public interface IEmailTemplateReader
    {
        Task<string> GetTemplate(string templateName);
    }
}