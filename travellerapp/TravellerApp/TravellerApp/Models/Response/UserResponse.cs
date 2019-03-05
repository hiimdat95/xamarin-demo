using TravellerApp.Models.Result;

namespace TravellerApp.Models.Response
{
    public class UserResponse
    {
        public string jsonrpc { get; set; }

        public int? id { get; set; }

        public UserResult result { get; set; }
    }
}