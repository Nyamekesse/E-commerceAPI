using RestSharp;
using RestSharp.Authenticators;
namespace E_commerceAPI.Services
{
    public class EmailSender
    {
        private static readonly string APIKey = Environment.GetEnvironmentVariable("MAIL_GUN_API_KEY")!;
        private static readonly string Domain = Environment.GetEnvironmentVariable("MAILGUN_DOMAIN")!;
        private const string BaseUri = "https://api.mailgun.net/v3";
        private const string SenderDisplayName = "PhoenixTechVault";
        private const string Tag = "new account";
        public static RestResponse SendEmail(string subject, string body, string receiver)
        {

            RestClient client = new(new RestClientOptions(BaseUri)
            {
                Authenticator = new HttpBasicAuthenticator("api", APIKey)
            });


            RestRequest request = new();
            request.AddParameter("domain", Domain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", $"{SenderDisplayName} <mailgun@{Domain}>");
            request.AddParameter("to", receiver);
            request.AddParameter("subject", subject);
            request.AddParameter("html", body);
            request.AddParameter("o:tag", Tag);
            request.Method = Method.Post;
            return client.Execute(request);
        }
    }
}
