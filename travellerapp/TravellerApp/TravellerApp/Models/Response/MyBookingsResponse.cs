using TravellerApp.Models.Result;

namespace TravellerApp.Models.Response
{
    internal class MyBookingsResponse
    {
        public string jsonrpc { get; set; }

        public int? id { get; set; }

        public ListBookingResult result { get; set; }
    }
}