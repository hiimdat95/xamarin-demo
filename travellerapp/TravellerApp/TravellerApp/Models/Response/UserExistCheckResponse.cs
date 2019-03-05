using TravellerApp.Models.Result;

namespace TravellerApp.Models.Response
{
    public class UserExistCheckResponse
    {
        public string jsonrpc { get; set; }

        public int? id { get; set; }

        public UserExistCheckResult result { get; set; }
    }
}