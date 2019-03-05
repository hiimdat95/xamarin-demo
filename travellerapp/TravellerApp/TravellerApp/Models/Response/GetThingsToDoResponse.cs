using TravellerApp.Models.Result;

namespace TravellerApp.Models.Response
{
    internal class GetThingsToDoResponse
    {
        public string jsonrpc { get; set; }

        public int? id { get; set; }

        public GetThingsToDoResult result { get; set; }
    }
}