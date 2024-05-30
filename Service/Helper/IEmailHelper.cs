using Data.ViewModel.Helper;

namespace Service.Helper
{
    public interface IEmailHelper
    {
        Task SendEmail(EmailRequest emailRequest);
    }
}