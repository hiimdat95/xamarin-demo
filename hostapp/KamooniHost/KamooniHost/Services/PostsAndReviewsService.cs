using KamooniHost.Constants;
using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models.Response;
using KamooniHost.Models.Result;
using KamooniHost.RestClient;
using KamooniHost.Utils;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace KamooniHost.Services
{
    public class PostsAndReviewsService : IPostsAndReviewsService
    {
        private readonly IRestClient restClient;

        public PostsAndReviewsService(IRestClient restClient)
        {
            this.restClient = restClient;
        }

        public async Task<HostResult> GetHostDetails()
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("token", Settings.CurrentHost.Token)
                    })
                })
            };

            HostResponse response = await restClient.PostAsync<HostResponse, JObject>(ApiURI.URL_MAIN + ApiURI.URI_GET_HOST_DETAILS, @params);

            return response?.Result;
        }

        public async Task<HostResult> GetPost(string email, string password)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("db", "kamooni"),
                        new JProperty("login", email.ToLower().Trim()),
                        new JProperty("password", StringFormatUtil.ToBase64(password))
                    })
                })
            };

            HostResponse response = await restClient.PostAsync<HostResponse, JObject>(ApiURI.URL_MAIN + ApiURI.URI_SIGN_IN, @params);

            return response?.Result;
        }
    }
}
