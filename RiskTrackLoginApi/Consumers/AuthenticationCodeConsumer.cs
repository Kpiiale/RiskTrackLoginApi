using MassTransit;
using RiskTrackLoginApi.Constracts;
using RiskTrackLoginApi.Services;

namespace RiskTrackLoginApi.Consumers
{
    public class AuthenticationCodeConsumer : IConsumer<AuthenticationCodeGenerated>
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthenticationCodeConsumer> _logger;

        public AuthenticationCodeConsumer(IEmailService emailService, ILogger<AuthenticationCodeConsumer> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<AuthenticationCodeGenerated> context)
        {
            var message = context.Message;
            _logger.LogInformation("Evento recibido para enviar código a {Email}", message.Email);

            await _emailService.SendVerificationEmailAsync(message.Email, message.Code);

            _logger.LogInformation("Correo con código de verificación enviado exitosamente a {Email}", message.Email);
        }
    }
}
