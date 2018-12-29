using System.Threading.Tasks;

namespace Climb.Services
{
    public class NullEmailService : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Task.CompletedTask;
        }

        public Task SendEmailConfirmationAsync(string email, string link)
        {
            return Task.CompletedTask;
        }

        public Task SendResetPasswordAsync(string email, string callbackUrl)
        {
            return Task.CompletedTask;
        }
    }
}