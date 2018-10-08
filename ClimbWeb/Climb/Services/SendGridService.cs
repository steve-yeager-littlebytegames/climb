using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

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
                ClickTracking = new ClickTracking { Enable = false }
            };

            var client = new SendGridClient(apiKey);
            await client.SendEmailAsync(msg);
        }
    }
}