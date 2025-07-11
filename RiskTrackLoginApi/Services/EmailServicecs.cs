using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace RiskTrackLoginApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendVerificationEmailAsync(string toEmail, string code)
        {
            // Lee la sección "Smtp", no "EmailSettings"
            var smtpSettings = _config.GetSection("Smtp");

            var email = new MimeMessage();

            // Usa las claves correctas: SenderName y User
            email.From.Add(new MailboxAddress(smtpSettings["SenderName"], smtpSettings["User"]));

            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "Your Verification Code";
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = $"<h1>Welcome to Our App!</h1><p>Your verification code is: <strong>{code}</strong></p><p>This code will expire in 10 minutes.</p>"
            };

            using var smtp = new SmtpClient();
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

            // Usa la clave correcta: Host
            await smtp.ConnectAsync(smtpSettings["Host"], int.Parse(smtpSettings["Port"]!), SecureSocketOptions.StartTls);

            // Usa las claves correctas: User y Password
            await smtp.AuthenticateAsync(smtpSettings["User"], smtpSettings["Password"]);

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
