using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TravellerApp.Models.Result
{
    public class TimeLineResult : BaseResult
    {
        [JsonProperty("time_line")]
        public List<TimeLine> TimeLines { get; set; }
    }
}