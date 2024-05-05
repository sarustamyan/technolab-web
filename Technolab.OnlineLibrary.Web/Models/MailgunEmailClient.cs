using FluentEmail.Core;
using FluentEmail.Mailgun;

namespace Technolab.OnlineLibrary.Web.Models
{
    public class MailgunEmailClient : IEmailClient
    {
        public MailgunEmailClient(IConfiguration configuration)
        {
            this.Settings = configuration.GetSection("Mailgun").Get<MailgunSettings>();
        }

        public void SendEmail(EmailMessage message)
        {
            var email = Email
                .From(emailAddress: Settings.FromAddress, name: message.From)
                .To(emailAddress: message.To)
                .Subject(message.Subject)
                .Body(message.Body);

            email.Sender = new MailgunSender(domainName: Settings.DomainName, apiKey: Settings.ApiKey);

            email.Send();
        }

        public MailgunSettings Settings { get; }
    }
}