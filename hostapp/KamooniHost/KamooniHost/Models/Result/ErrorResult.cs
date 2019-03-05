using Newtonsoft.Json;

namespace KamooniHost.Models.Result
{
    public class ErrorResult
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("data")]
        public ErrorData Data { get; set; }

        [JsonProperty("http_status")]
        public int HttpStatus { get; set; }
    }

    public class ErrorData
    {
        [JsonProperty("debug")]
        public string Debug { get; set; }

        [JsonProperty("exception_type")]
        public string ExceptionType { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("arguments")]
        public dynamic Arguments { get; set; }
    }
}