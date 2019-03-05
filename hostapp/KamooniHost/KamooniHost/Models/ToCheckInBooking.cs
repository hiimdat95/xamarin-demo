using Newtonsoft.Json;
using System;

namespace KamooniHost.Models
{
    public class ToCheckInBooking : BaseModel
    {
        private string bookingId;

        [JsonProperty("id")]
        public string BookingId { get => bookingId; set => SetProperty(ref bookingId, value); }

        private string bookingRef;

        [JsonProperty("booking_ref")]
        public string BookingRef { get => bookingRef; set => SetProperty(ref bookingRef, value); }

        private Customer customer;

        [JsonProperty("customer")]
        public Customer Customer { get => customer; set => SetProperty(ref customer, value); }

        private int totalAdults;

        [JsonProperty("adults")]
        public int TotalAdults { get => totalAdults; set => SetProperty(ref totalAdults, value); }

        private DateTime startDate;

        [JsonProperty("start_date")]
        public DateTime StartDate { get => startDate; set => SetProperty(ref startDate, value); }

        private DateTime endDate;

        [JsonProperty("end_date")]
        public DateTime EndDate { get => endDate; set => SetProperty(ref endDate, value); }
    }
}