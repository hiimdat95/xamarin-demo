using KamooniHost.Models.Result;
using Newtonsoft.Json;

namespace KamooniHost.Models.Response {
	public class CheckHostVerifyResponse
    {
		[JsonProperty("jsonrpc")]
		public string Jsonrpc { get; set; }

		[JsonProperty("id")]
		public object Id { get; set; }

		[JsonProperty("result")]
		public CheckHostVerifyResult Result { get; set; }
	}
}
