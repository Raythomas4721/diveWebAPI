namespace diveWebAPI.Services
{
    public interface IEmailService
    {
        Task SendVerificationCodeAsync(string email, string code);
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
    }
}