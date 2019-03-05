using KamooniHost.Models.Result;
using Newtonsoft.Json;

namespace KamooniHost.Models.Profile
{
    public class ProfileResult : BaseResult
    {
        [JsonProperty("Profile")]
        public Profile Profile { get; set; }
    }
}