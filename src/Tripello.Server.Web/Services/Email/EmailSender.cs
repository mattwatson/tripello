using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SendGrid;
using SendGrid.Helpers.Mail;
using Tripello.Server.Web.Data;

namespace Tripello.Server.Web.Services.Email
{
    public class AuthMessageSenderOptions
    {
        public string SendGridUser { get; set; }
        public string SendGridKey { get; set; }
        public string FromAddress { get; set; }

        public bool IsValid
        {
            get {
                return !String.IsNullOrEmpty(SendGridUser)
                    && !String.IsNullOrEmpty(SendGridKey)
                    && !String.IsNullOrEmpty(FromAddress);
            }
        }
    }    

    public class EmailSender : IEmailSender
    {
        private readonly AuthMessageSenderOptions options;

        public EmailSender(AuthMessageSenderOptions options)
        {
            this.options = options;
            if (String.IsNullOrEmpty(options.SendGridUser))
            {
                throw new ArgumentException("Must specify AuthMessageSender.SendGridUser if using AuthMessageSender");
            }

            if (String.IsNullOrEmpty(options.SendGridKey))
            {
                throw new ArgumentException("Must specify AuthMessageSender.SendGridKey if using AuthMessageSender");
            }

            if (String.IsNullOrEmpty(options.FromAddress))
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

    public static class EmailExtenisions
    {
        public static IServiceCollection AddAuthenticationWithOptions(this IServiceCollection services, IConfiguration configuration)
        {
            var authMessageSenderOptions = configuration.GetSection("AuthMessageSender").Get<AuthMessageSenderOptions>();
            if (authMessageSenderOptions != null && authMessageSenderOptions.IsValid)
            {
                services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<ApplicationDbContext>();
                services.AddTransient<IEmailSender, EmailSender>();
                services.AddSingleton<AuthMessageSenderOptions>(authMessageSenderOptions);
            }
            else
            {
                services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                    .AddEntityFrameworkStores<ApplicationDbContext>();
            }

            return services;
        }
    }

}