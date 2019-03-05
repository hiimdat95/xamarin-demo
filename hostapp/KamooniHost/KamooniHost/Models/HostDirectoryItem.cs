using Newtonsoft.Json;

namespace KamooniHost.Models {

	public class HostDirectoryItem : BaseModel
    {
		private int id;

		[JsonProperty("id")]
		public int Id { get => id; set => SetProperty(ref id, value); }

		private string name;

		[JsonProperty("name")]
		public string Name { get => name; set => SetProperty(ref name, value); }

		private string email;

		[JsonProperty("email")]
		public string Email { get => email; set => SetProperty(ref email, value); }

		private string mobile;

		[JsonProperty("mobile")]
		public string Mobile {
			get => mobile;
			set {
				if (value != "false") 
					SetProperty(ref mobile, value);
				else
					SetProperty(ref mobile, "(Not Listed)");
			}
		}
	}
}
