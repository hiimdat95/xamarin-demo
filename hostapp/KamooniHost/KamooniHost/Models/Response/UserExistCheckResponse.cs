using KamooniHost.Models.Result;
using Newtonsoft.Json;

namespace KamooniHost.Models.Response {
	public class UserExistCheckResponse
    {
		[JsonProperty("jsonrpc")]
		public string Jsonrpc { get; set; }

		[JsonProperty("id")]
		public object Id { get; set; }

		[JsonProperty("result")]
		public UserExistCheckResult Result { get; set; }
	}
}
