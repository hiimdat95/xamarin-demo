using Newtonsoft.Json;
using System.Collections.Generic;

namespace KamooniHost.Models.Result
{
    public class GuestBillsResult
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("bills")]
        public List<GuestBill> GuestBills { get; set; }
    }
}