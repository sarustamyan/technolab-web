namespace Technolab.OnlineLibrary.Web.Models
{
    public interface IEmailClient
    {
        void SendEmail(EmailMessage message);
    }
}