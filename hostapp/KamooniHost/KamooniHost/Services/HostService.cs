using KamooniHost.Constants;
using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using KamooniHost.Models.Response;
using KamooniHost.Models.Result;
using KamooniHost.RestClient;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KamooniHost.Services
{
    public class HostService : IHostService
    {
        private readonly IRestClient restClient;

        public HostService(IRestClient restClient)
        {
            this.restClient = restClient;
        }

        public async Task<List<string>> GetListDatabase(string url)
        {
            DatabasesResponse response = await restClient.PostAsync<DatabasesResponse, JObject>(url + ApiURI.URI_GET_DATABASE_LIST, new JObject());

            return response?.Result;
        }

        public async Task<HostTokenResult> GetHostToken(string url, string db, string user, string password)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("db", db),
                        new JProperty("login", user),
                        new JProperty("password", password)
                    })
                })
            };

            HostTokenResponse response = await restClient.PostAsync<HostTokenResponse, JObject>(url + ApiURI.URI_GET_HOST_TOKEN, @params);

            return response?.Result;
        }

        public async Task<HostResult> GetHostDetails(string token)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("token", token)
                    })
                })
            };

            HostResponse response = await restClient.PostAsync<HostResponse, JObject>(ApiURI.URL_MAIN + ApiURI.URI_GET_HOST_DETAILS, @params);

            return response?.Result;
        }

        public async Task<PostResult> UpdateHost(Host host)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("token", Settings.CurrentHost.Token)
                    }),
                    new JProperty("host", new JObject()
                    {
                        new JProperty("name", host.Name),
                        new JProperty("email", host.Email),
                        new JProperty("mobile", host.Mobile),
                        new JProperty("street", host.Street),
                        new JProperty("street2", host.Street2),
                        new JProperty("city", host.City),
                        new JProperty("zip", host.Zip),
                        new JProperty("state_id", host.State?.Id),
                        new JProperty("country", host.Country?.Code),
                        new JProperty("image", host.Image),
                        new JProperty("is_activity", host.IsActivity),
                        new JProperty("is_accommodation", host.IsAccommodation),
                        new JProperty("is_transport", host.IsTransport),
                        new JProperty("terms", host.Terms),
                        new JProperty("url_terms", host.UrlTerms),
                        new JProperty("partner_latitude", host.Latitude),
                        new JProperty("partner_longitude", host.Longitude)
                    })
                })
            };

            PostResponse response = await restClient.PostAsync<PostResponse, JObject>(ApiURI.URL_MAIN + ApiURI.URI_UPDATE_HOST, @params);

            return response?.Result;
        }
    }
}