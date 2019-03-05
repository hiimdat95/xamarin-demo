using Newtonsoft.Json;
using System.Collections.Generic;

namespace KamooniHost.Models.Result
{
    public class ToCheckInResult
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("bookings")]
        public List<ToCheckInBooking> Bookings { get; set; }
    }
}