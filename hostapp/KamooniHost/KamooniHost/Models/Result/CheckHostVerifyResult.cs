using Newtonsoft.Json;

namespace KamooniHost.Models.Result {
	public class CheckHostVerifyResult
    {
		[JsonProperty("success")]
		public bool Success { get; set; }

		[JsonProperty("message")]
		public string Message { get; set; }

		[JsonProperty("host")]
		public HostToVerify HostToVerify { get; set; }
	}
}
