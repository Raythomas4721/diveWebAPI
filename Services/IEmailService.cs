namespace diveWebAPI.Services // 調整命名空間以符合你的專案結構
{
    public interface IEmailService
    {
        Task SendVerificationCodeAsync(string email, string code);
    }
}