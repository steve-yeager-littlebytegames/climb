using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Climb.Services
{
    public class SendGridService : IEmailSender
    {
        private readonly string apiKey;

        public SendGridService(IConfiguration configuration)
        {
            apiKey = configuration.GetSection("Email")["Key"];
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var msg = new SendGridMessage
            {
                From = new EmailAddress("developer@littlebytegames.com", "Climb"),
                Subject = $"Climb - {subject}",
                PlainTextContent = message,
                HtmlContent = message,
            };
            msg.AddTo(email);

            msg.TrackingSettings = new TrackingSettings
            {
                ClickTracking = new ClickTracking {Enable = false}
            };

            var client = new SendGridClient(apiKey);
            await client.SendEmailAsync(msg);
        }

        public Task SendEmailConfirmationAsync(string email, string link)
        {
            return SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(link)}'>clicking here</a>.");
        }

        public Task SendResetPasswordAsync(string email, string callbackUrl)
        {
            return SendEmailAsync(email, "Reset Password",
                $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        }
    }
}