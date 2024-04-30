using System;
using System.IO;
using RestSharp;
using RestSharp.Authenticators;

namespace Technolab.OnlineLibrary.Web.Models
{
    public class Email
    {
        public void SendEmail()
        {
            Console.WriteLine(SendSimpleMessage().Content.ToString());
        }
        public RestResponse SendSimpleMessage()
        {
            RestClient client = new RestClient();
            client.Options.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator = new HttpBasicAuthenticator("api", "YOUR_API_KEY");
            RestRequest request = new RestRequest();
            request.AddParameter("domain", "sandbox71fd493341ab4787b439b88612446191.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "mailgun@sandbox71fd493341ab4787b439b88612446191.mailgun.org");
            request.AddParameter("to", "bar@example.com");
            request.AddParameter("subject", "Hello");
            request.AddParameter("text", "Testing some Mailgun awesomeness!");
            request.Method = Method.Post;
            return client.Execute(request);
        }
    }
}
