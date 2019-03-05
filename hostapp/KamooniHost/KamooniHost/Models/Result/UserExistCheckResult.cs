using Newtonsoft.Json;

namespace KamooniHost.Models.Result {
	public class UserExistCheckResult : BaseResult
    {
		[JsonProperty("user_exist")]
		public bool UserExists { get; set; }
	}
}
