using TravellerApp.Models.Result;

namespace TravellerApp.Models.Response
{
    internal class CurrentlyAtResponse
    {
        public string jsonrpc { get; set; }

        public int? id { get; set; }

        public CurrentlyAtResult result { get; set; }
    }
}