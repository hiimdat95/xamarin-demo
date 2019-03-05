using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TravellerApp.Models.Response
{
    class CheckStatusResponse : BaseResponse
    {
        [JsonProperty("result")]
        public Result Result { get; set; }
    }

    class Result
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("traveller")]
        public CheckStatus Status { get; set; }
    }

    class CheckStatus
    {
        [JsonProperty("total_visits")]
        public string TotalVisits { get; set; }

        [JsonProperty("total_ratings")]
        public string TotalRatings { get; set; }

        [JsonProperty("total_give_a_minute")]
        public string TotalGiveAMinute { get; set; }

        [JsonProperty("total_kamoonity")]
        public string TotalKamoonity { get; set; }

        [JsonProperty("total_points")]
        public string TotalPoints { get; set; }
    }

}
