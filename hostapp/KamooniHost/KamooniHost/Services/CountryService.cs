using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using KamooniHost.RestClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KamooniHost.Services
{
    public class CountryService : ICountryService
    {
        private readonly IRestClient restClient;

        public CountryService(IRestClient restClient)
        {
            this.restClient = restClient;
        }
        
        public async Task<StatesResult> GetStates(string countryCode)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("country_iso", countryCode)
                })
            };

            StatesResponse response = await restClient.PostAsync<StatesResponse, JObject>(Constants.ApiURI.URL_MAIN + Constants.ApiURI.URI_GET_STATES, @params);

            return response?.Result;
        }
    }
}
