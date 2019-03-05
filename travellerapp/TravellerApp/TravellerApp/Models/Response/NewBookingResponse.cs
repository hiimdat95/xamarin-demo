using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TravellerApp.Models.Response
{
    class NewBookingResponse : BaseResponse
    {
        [JsonProperty("result")]
        public NewBookingResult Result { get; set; }
    }

    class NewBookingResult
    {
        [JsonProperty("kamooni_box_id")]
        public int kamooni_box_id { get; set; }

        [JsonProperty("host_id")]
        public int host_id { get; set; }

        [JsonProperty("peach_payment_link_url")]
        public string peach_payment_link_url { get; set; }

        [JsonProperty("success")]
        public bool success { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("amount")]
        public double amount { get; set; }

        [JsonProperty("traveller_id")]
        public int traveller_id { get; set; }

        [JsonProperty("booking_token")]
        public string booking_token { get; set; }

    }
  }
