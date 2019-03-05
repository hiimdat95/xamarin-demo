using KamooniHost.Constants;
using KamooniHost.IServices;
using KamooniHost.Models;
using KamooniHost.Models.Response;
using KamooniHost.Models.Result;
using KamooniHost.RestClient;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace KamooniHost.Services
{
    internal class SignUpService : ISignUpService
    {
        private readonly IRestClient restClient;

        public SignUpService(IRestClient restClient)
        {
            this.restClient = restClient;
        }

        public async Task<HostDirectoryResult> GetHostDirectory()
        {
            JObject @params = new JObject();
            HostDirectoryResponse response = await restClient.PostAsync<HostDirectoryResponse, JObject>(ApiURI.URL_MAIN + ApiURI.URI_GET_HOST_DIRECTORY, @params);
            return response?.Result;
        }

        public async Task<SuccessCheckResult> SendPortalInvite(int hostId)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("host", new JObject()
                    {
                        new JProperty("id", hostId)
                    })
                })
            };

            SuccessCheckResponse response = await restClient.PostAsync<SuccessCheckResponse, JObject>(ApiURI.URL_MAIN + ApiURI.URI_SEND_PORTAL_INVITE, @params);

            return response?.Result;
        }

        public async Task<HostTokenResult> CreateHost(HostToCreate hostToCreate)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("host", new JObject()
                    {
                        new JProperty("name", hostToCreate.Name),
                        new JProperty("email", hostToCreate.Email),
                        new JProperty("mobile", hostToCreate.Mobile),
                        new JProperty("street", hostToCreate.Street),
                        new JProperty("street2", hostToCreate.Street2),
                        new JProperty("city", hostToCreate.City),
                        new JProperty("zip", hostToCreate.Zip),
                        new JProperty("state_id", hostToCreate.State?.Id),
                        new JProperty("country", hostToCreate.Country.Code),
                        new JProperty("image", hostToCreate.Image),
                        new JProperty("is_activity", hostToCreate.IsActivity),
                        new JProperty("is_accommodation", hostToCreate.IsAccommodation),
                        new JProperty("is_transport", hostToCreate.IsTransport),
                        new JProperty("terms", hostToCreate.Terms),
                        new JProperty("url_terms", hostToCreate.UrlTerms),
                        new JProperty("partner_latitude", hostToCreate.Latitude),
                        new JProperty("partner_longitude", hostToCreate.Longitude)
                    })
                })
            };

            HostTokenResponse response = await restClient.PostAsync<HostTokenResponse, JObject>(ApiURI.URL_MAIN + ApiURI.URI_CREATE_HOST, @params);

            return response?.Result;
        }

        public async Task<CheckHostVerifyResult> CheckHostVerify(string hostToken)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("host", new JObject()
                    {
                        new JProperty("token", hostToken)
                    })
                })
            };

            CheckHostVerifyResponse response = await restClient.PostAsync<CheckHostVerifyResponse, JObject>(ApiURI.URL_MAIN + ApiURI.URI_CHECK_HOST_VERIFY, @params);

            return response?.Result;
        }
    }
}