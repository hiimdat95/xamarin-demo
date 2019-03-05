using Newtonsoft.Json;

namespace TravellerApp.Models.Response
{
    public class BaseResponse
    {
        [JsonProperty("jsonrpc")]
        public string Jsonrpc { get; set; }

        [JsonProperty("id")]
        public int? Id { get; set; }
    }

    public class BaseResponse<T>
    {
        [JsonProperty("jsonrpc")]
        public string Jsonrpc { get; set; }

        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("result")]
        public T Result { get; set; }
    }
}