using Newtonsoft.Json;
using System;

namespace TravellerApp.Models.Result
{
    public class TimeLine
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("date_start")]
        public DateTime? DateStart { get; set; }

        [JsonProperty("date_end")]
        public DateTime? DateEnd { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}