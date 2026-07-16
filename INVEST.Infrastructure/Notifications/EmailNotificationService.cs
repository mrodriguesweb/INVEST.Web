using INVEST.Application.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace INVEST.Infrastructure.Notifications
{
    public class EmailNotificationService : INotificationService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailNotificationService> _logger;

        public EmailNotificationService(IConfiguration config, ILogger<EmailNotificationService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendAsync(string title, string message, CancellationToken ct)
        {
            var smtpServer = _config["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
            var port = int.Parse(_config["EmailSettings:Port"] ?? "587");
            var senderEmail = _config["EmailSettings:SenderEmail"];
            var senderPassword = _config["EmailSettings:SenderPassword"];

            if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword))
            {
                _logger.LogError("Credenciais de e-mail ausentes no appsettings/variáveis de ambiente.");
                throw new InvalidOperationException("Falta configuração de e-mail.");
            }

            try
            {
                using var client = new SmtpClient(smtpServer, port)
                {
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, "Sistema INVEST"),
                    Subject = title,
                    Body = message,
                    IsBodyHtml = false // Mude para true se quiser mandar tags <b>, <br>, etc. no futuro
                };

                // Para esse teste, estamos mandando o e-mail de você para você mesmo!
                mailMessage.To.Add(senderEmail);

                _logger.LogInformation("Conectando ao SMTP do Google para enviar e-mail...");

                await client.SendMailAsync(mailMessage, ct);

                _logger.LogInformation("E-mail REAL enviado com sucesso para {Email}!", senderEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha crítica ao enviar e-mail via SMTP.");
                // IMPORTANTE: Nós lançamos o erro novamente (throw) para que o Worker 
                // entenda que falhou e aplique a regra de Nack (Requeue ou DLQ) que programamos!
                throw;
            }
        }
    }
}