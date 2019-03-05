using Newtonsoft.Json;
using System.Collections.Generic;

namespace KamooniHost.Models.Result
{
    public class CheckedInGuestsResult : BaseModel
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("guest_list")]
        public List<CheckedInGuest> GuestList { get; set; }
    }
}