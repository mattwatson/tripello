using System;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Tripello.Server.Web.Services.Email
{
    public class AuthMessageSenderOptions
    {
        public string SendGridUser { get; set; }
        public string SendGridKey { get; set; }
        public string FromAddress { get; set;}
    }

    public class EmailSender : IEmailSender
    {
        private readonly AuthMessageSenderOptions options;

        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            options = optionsAccessor.Value;
            if (String.Is.IsNullOrEmpty(options.SendGridUser))
            {
                throw new ArgumentException("Must specify AuthMessageSender.SendGridUser if using AuthMessageSender");
            }

            if (String.Is.IsNullOrEmpty(options.SendGridKey))
            {
                throw new ArgumentException("Must specify AuthMessageSender.SendGridKey if using AuthMessageSender");
            }

            if (String.Is.IsNullOrEmpty(options.FromAddress))
            {
                throw new ArgumentException("Must specify AuthMessageSender.FromAddress if using AuthMessageSender");
            }
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(subject, message, email);
        }

        private Task Execute(string subject, string message, string email)
        {
            var client = new SendGridClient(options.SendGridKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(options.FromAddress, options.SendGridUser),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);
        }
    }
}