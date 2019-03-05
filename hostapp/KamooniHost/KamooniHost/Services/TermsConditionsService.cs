using KamooniHost.Constants;
using KamooniHost.IServices;
using KamooniHost.Models;
using KamooniHost.Models.Result;
using KamooniHost.RestClient;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace KamooniHost.Services
{
    public class TermsConditionsService : ITermsConditionsService
    {
        private readonly IRestClient restClient;

        public TermsConditionsService(IRestClient restClient)
        {
            this.restClient = restClient;
        }

        public async Task<PostResult> NewTermsConditions(Host Host)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        //new JProperty("login", Settings.CurrentHost.Email),
                        new JProperty("usr_token", Host.Token)
                    }),
                    new JProperty("host", new JObject()
                    {
                        new JProperty("terms", Host.Terms),
                        new JProperty("url_terms", Host.UrlTerms)
                    })
                })
            };

            PostResult response = await restClient.PostAsync<PostResult, JObject>(ApiURI.URL_MAIN + ApiURI.UPDATE_TERMS, @params);

            return response;
        }
    }
}