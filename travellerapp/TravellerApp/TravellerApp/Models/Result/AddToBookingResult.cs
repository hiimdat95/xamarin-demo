using Newtonsoft.Json;
using System.Collections.Generic;

namespace TravellerApp.Models.Result
{
    public class AddToBookingResult : BaseResult
    {
        [JsonProperty("manual_add")]
        public bool ManualAdd { get; set; }

        [JsonProperty("guest_list")]
        public List<AddToBookingGuest> Guests { get; set; }
    }
}