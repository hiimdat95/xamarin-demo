using Newtonsoft.Json;
using System.Collections.Generic;

namespace KamooniHost.Models.Result {
	public class HostDirectoryResult : BaseResult
    {
		[JsonProperty("found")]
		public int Found { get; set; }

		[JsonProperty("hosts")]
		public List<HostDirectoryItem> Hosts { get; set; }
	}
}
