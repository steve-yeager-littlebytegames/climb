using System.Threading.Tasks;

namespace Climb.Services
{
    public class NullEmailService : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Task.CompletedTask;
        }
    }
}