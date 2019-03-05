using TravellerApp.Models.Result;

namespace TravellerApp.Models.Response
{
    public class SuccessCheckResponse
    {
        public string jsonrpc { get; set; }

        public int? id { get; set; }

        public SuccessCheckResult result { get; set; }
    }
}