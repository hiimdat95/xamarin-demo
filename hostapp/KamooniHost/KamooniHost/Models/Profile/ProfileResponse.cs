using KamooniHost.Models.Response;
using Newtonsoft.Json;

namespace KamooniHost.Models.Profile
{
    public class ProfileResponse : BaseResponse
    {
        [JsonProperty("result")]
        public ProfileResult Result { get; set; }
    }
}