using TravellerApp.Models.Result;

namespace TravellerApp.Models.Response
{
    class AllPostsResponse
    {
        public string jsonrpc { get; set; }

        public int? id { get; set; }

        public AllPostsResult result { get; set; }
    }
}
