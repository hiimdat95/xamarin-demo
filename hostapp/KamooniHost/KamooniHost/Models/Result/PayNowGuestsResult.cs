using Newtonsoft.Json;
using System.Collections.Generic;

namespace KamooniHost.Models.Result
{
    public class PayNowGuestsResult
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("guest_id")]
        public int MainGuestId { get; set; }

        [JsonProperty("total")]
        public double Total { get; set; }

        [JsonProperty("paid")]
        public double Paid { get; set; }

        [JsonProperty("balance")]
        public double Balance { get; set; }

        [JsonProperty("guest_list")]
        public List<CheckedInGuest> ListGuest { get; set; }
    }
}