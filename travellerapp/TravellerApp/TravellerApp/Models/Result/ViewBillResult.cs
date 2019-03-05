using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TravellerApp.Models.Result
{
    public class ViewBillResult : BaseResult
    {
        [JsonProperty("booking_id")]
        public int BookingId { get; set; }

        [JsonProperty("guest_id")]
        public int GuestId { get; set; }

        [JsonProperty("host_url")]
        public Uri HostUrl { get; set; }

        [JsonProperty("booking_token")]
        public string BookingToken { get; set; }

        [JsonProperty("bill_lines")]
        public List<BillLine> BillLines { get; set; }

        [JsonProperty("total_booking_balance")]
        public long TotalBookingBalance { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("paid")]
        public long Paid { get; set; }

        [JsonProperty("balance")]
        public long Balance { get; set; }

        [JsonProperty("tip")]
        public long Tip { get; set; }
        
        [JsonProperty]
        public List<GroupedBillLine> GroupedBillLines { get; set; }
    }

    public class GroupedBillLine : BaseModel
    {
        [JsonProperty("categ_id")]
        public string CategId { get; set; }

        [JsonProperty("bill_lines")]
        public List<BillLine> BillLines { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }
    }
}