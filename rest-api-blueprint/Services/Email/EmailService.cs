using rest_api_blueprint.Models.Email;
using rest_api_blueprint.Models.Pwa;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace rest_api_blueprint.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<SmtpOptions> _smtpOptions;
        private readonly IOptions<PwaOptions> _pwaOptions;

        public EmailService(IOptions<SmtpOptions> smtpOptions, IOptions<PwaOptions> pwaOptions)
        {
            _smtpOptions = smtpOptions;
            _pwaOptions = pwaOptions;
        }

        public async Task SendEmailConfirmationMail(string userId, string username, string email, string token)
        {
            string subject = "Confirm your email address for asgoodaspros.com";
            string destination = $"{_pwaOptions.Value.Uri}{_pwaOptions.Value.PathConfirmEmail}?userName={username}&token={token}";
            string body = $"<p>Dear {username},</p><p>Your confirmation token is:</p><p><pre>{token}</pre></p><p>Please use this token to register at <a href=\"{destination}\">https://{destination}</a></p>";

            await SendEmail(email, subject, body);
        }

        public async Task SendResetPasswordMail(string username, string email, string token)
        {
            string subject = "Reset your password on asgoodaspros.com";
            string destination = $"{_pwaOptions.Value.Uri}{_pwaOptions.Value.PathResetPassword}?userName={username}&token={token}";
            string body = $"<p>Dear {username},</p><p>Your password reset token is:</p><p><pre>{token}</pre></p><p>Please use this token to reset your password at <a href=\"{destination}\">{destination}</a></p>";

            await SendEmail(email, subject, body);
        }

        public async Task SendAccountActivatedMail(string username, string email)
        {
            string subject = "Account unlocked on asgoodaspros.com";
            string body = $"<p>Dear {username},</p><p>your account has been unlocked and you can now login at:</p><p><a href=\"https://asgoodaspros.com\">https://asgoodaspros.com</a></p>";

            await SendEmail(email, subject, body);
        }

        public async Task SendAccountDeactivatedMail(string username, string email)
        {
            string subject = "Account locked on asgoodaspros.com";
            string body = $"<p>Dear {username},</p><p>your account has been locked and you can't login anymore at:</p><p><a href=\"https://asgoodaspros.com\">https://asgoodaspros.com</a></p>";

            await SendEmail(email, subject, body);
        }

        private async Task SendEmail(string email, string subject, string body)
        {
            string from = $"{_smtpOptions.Value.SenderName} <{_smtpOptions.Value.SenderEmail}>";
            MailMessage mailMessage = new MailMessage(from, email, subject, body);

            mailMessage.IsBodyHtml = true;
            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            using (SmtpClient mailClient = new SmtpClient()
            {
                Host = _smtpOptions.Value.Host,
                Port = _smtpOptions.Value.Port,
                UseDefaultCredentials = false,
                EnableSsl = true,
                Credentials = new NetworkCredential(_smtpOptions.Value.Username, _smtpOptions.Value.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network
            })
            {
                await mailClient.SendMailAsync(mailMessage);
            }
        }
    }
}
