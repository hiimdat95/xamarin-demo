using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models.Response;
using KamooniHost.Models.Result;
using KamooniHost.RestClient;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace KamooniHost.Services
{
    public class StatsService : IStatsService
    {
        private readonly IRestClient restClient;

        public StatsService(IRestClient restClient)
        {
            this.restClient = restClient;
        }

        public async Task<RevenueResult> GetRevenue()
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

            RevenueResponse response = await restClient.PostAsync<RevenueResponse, JObject>(Settings.CurrentHost.Url + Constants.ApiURI.URI_GET_REVENUE, @params);

            return response?.Result;
        }
    }
}