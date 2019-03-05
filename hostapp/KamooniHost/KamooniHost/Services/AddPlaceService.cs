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
    public class AddPlaceService : IAddPlaceService
    {
        private readonly IRestClient restClient;

        public AddPlaceService(IRestClient restClient)
        {
            this.restClient = restClient;
        }

    }
}
