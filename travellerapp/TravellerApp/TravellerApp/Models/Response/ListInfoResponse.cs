using TravellerApp.Models.Result;

namespace TravellerApp.Models.Response
{
    public class ListInfoResponse
    {
        public string jsonrpc { get; set; }

        public int? id { get; set; }

        public ListInfoResult result { get; set; }
    }
}