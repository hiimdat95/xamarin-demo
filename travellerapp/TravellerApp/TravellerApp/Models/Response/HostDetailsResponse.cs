using TravellerApp.Models.Result;

namespace TravellerApp.Models.Response
{
    public class HostDetailsResponse
    {
        public string jsonrpc { get; set; }

        public int? id { get; set; }

        public HostDetailsResult result { get; set; }
    }
}