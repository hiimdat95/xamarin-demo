using Newtonsoft.Json;
using System.Collections.Generic;

namespace KamooniHost.Models.Result
{
    public class GuestBookResult : BaseResult
    {
        [JsonProperty("guests")]
        public List<GuestBook> GuestBooks { get; set; }
    }
}