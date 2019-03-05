using Newtonsoft.Json;
using System.Collections.Generic;

namespace KamooniHost.Models.Result
{
    public class GuestProfileResult : BaseResult
    {
        [JsonProperty("guest")]
        public List<GuestProfile> GuestProfiles { get; set; }
    }
}