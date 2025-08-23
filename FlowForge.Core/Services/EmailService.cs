using FlowForge.Core.DTO;
using FlowForge.Core.ServiceContracts;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace FlowForge.Core.Services
{
    public class EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> _logger) : IEmailSender
    {
        private readonly EmailSettings _emailSettings = emailSettings.Value;

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = true,
                Timeout = 20000
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (SmtpException ex)
            {
                _logger.LogError($"SMTP error: {ex.StatusCode} - {ex.Message} - {ex.InnerException?.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"General error: {ex.Message} - {ex.InnerException?.Message}");
                throw;
            }
        }
    }
}
