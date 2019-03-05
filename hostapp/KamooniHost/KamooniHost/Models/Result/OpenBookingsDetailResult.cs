using Newtonsoft.Json;
using System.Collections.Generic;

namespace KamooniHost.Models.Result
{
    public class OpenBookingsDetailResult
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("payment_list")]
        public List<PastPayment> ListPastPayment { get; set; }

        [JsonProperty("room_list")]
        public List<CheckedInRoom> ListRoom { get; set; }

        [JsonProperty("extra_list")]
        public List<ExtraBought> ListExrta { get; set; }
    }
}