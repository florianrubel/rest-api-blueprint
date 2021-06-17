using System.Threading.Tasks;

namespace rest_api_blueprint.Services.Email
{
    public interface IEmailService
    {
        Task SendAccountActivatedMail(string username, string email);
        Task SendAccountDeactivatedMail(string username, string email);
        Task SendEmailConfirmationMail(string userId, string username, string email, string token);
        Task SendResetPasswordMail(string username, string email, string token);
    }
}