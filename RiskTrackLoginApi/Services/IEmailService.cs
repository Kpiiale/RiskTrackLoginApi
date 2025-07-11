namespace RiskTrackLoginApi.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string toEmail, string code);
    }
}
