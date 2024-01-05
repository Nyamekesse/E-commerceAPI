using RestSharp;
using RestSharp.Authenticators;
namespace E_commerceAPI.Services
{
    public class EmailSender(IConfiguration configuration)
    {
        private readonly string Domain = configuration.GetValue<string>("EmailConfigurationOptions:MailgunDomain")!;
        private static readonly string APIKey = Environment.GetEnvironmentVariable("MAIL_GUN_API_KEY")!;
        private readonly string BaseUri = configuration.GetValue<string>("EmailConfigurationOptions:BaseUri")!;
        private readonly string SenderDisplayName = configuration.GetValue<string>("EmailConfigurationOptions:DisplayName")!;
        private readonly string Tag = configuration.GetValue<string>("EmailConfigurationOptions:Tag")!;

        public RestResponse SendEmail(string subject, string body, string receiver)
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
