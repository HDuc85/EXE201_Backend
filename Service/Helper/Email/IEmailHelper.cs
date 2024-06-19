using Data.ViewModel.Helper;

namespace Service.Helper.Email
{
    public interface IEmailHelper
    {
        Task SendEmail(EmailRequest emailRequest);
    }
}