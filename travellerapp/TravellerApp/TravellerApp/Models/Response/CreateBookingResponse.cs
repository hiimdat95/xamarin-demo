using TravellerApp.Models.Result;

namespace TravellerApp.Models.Response
{
    public class CreateBookingResponse
    {
        public string jsonrpc { get; set; }

        public int? id { get; set; }

        public CreateBookingResult result { get; set; }
    }
}