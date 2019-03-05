using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace KamooniHost.RestClient
{
    public class AuthenticationMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty("ApiKey"))
            {
                request.Headers.Add("X-DreamFactory-Api-Key", "ApiKey");
            }

            if (!string.IsNullOrEmpty("SessionToken"))
            {
                request.Headers.Add("X-DreamFactory-Session-Token", "SessionToken");
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}