using Newtonsoft.Json;
using System.Reflection;
using Xamarin.Forms;

namespace KamooniHost.Models
{
    public class CheckInFormItem : BaseModel
    {
        private int id;

        [JsonProperty("id")]
        public int Id { get => id; set => SetProperty(ref id, value); }

        private string name;

        [JsonProperty("name")]
        public string Name { get => name; set => SetProperty(ref name, value, onChanged: () => OnPropertyChanged(nameof(IsValid))); }

        private string email;

        [JsonProperty("email")]
        public string Email { get => email; set => SetProperty(ref email, value, onChanged: () => OnPropertyChanged(nameof(IsValid))); }

        private string mobile;

        [JsonProperty("mobile")]
        public string Mobile { get => mobile; set => SetProperty(ref mobile, value, onChanged: () => OnPropertyChanged(nameof(IsValid))); }

        private string passportId;

        [JsonProperty("passport_id")]
        public string PassportId { get => passportId; set => SetProperty(ref passportId, value, onChanged: () => OnPropertyChanged(nameof(IsValid))); }

        private string passportImage;

        [JsonProperty("passport_image")]
        public string PassportImage { get => passportImage; set => SetProperty(ref passportImage, value, onChanged: () => OnPropertyChanged(nameof(IsPassportImageTaked), nameof(IsValid))); }

        public bool IsPassportImageTaked => !string.IsNullOrWhiteSpace(PassportImage);

        private string room;

        [JsonProperty("room")]
        public string Room { get => room; set => SetProperty(ref room, value, onChanged: () => OnPropertyChanged(nameof(IsValid))); }

        private string countryISO;

        [JsonProperty("country_iso")]
        public string CountryISO { get => countryISO; set => SetProperty(ref countryISO, value); }

        private string countryName;

        [JsonProperty("country_name")]
        public string CountryName { get => countryName; set => SetProperty(ref countryName, value); }

        private string countryFlag;

        [JsonProperty("country_flag")]
        public string CountryFlag { get => countryFlag; set => SetProperty(ref countryFlag, value, onChanged: () => OnPropertyChanged(nameof(CountryFlagImage), nameof(IsValid))); }

        public ImageSource CountryFlagImage
        {
            get
            {
                try
                {
                    if (CountryFlag == null)
                        return null;

                    var Assembly = typeof(App).GetTypeInfo().Assembly;

                    var strAssemblyName = Assembly.GetName().Name;
                    var resource = "AppResources.Assets";
                    var imageSource = ImageSource.FromResource($"{strAssemblyName}.{resource}.{CountryFlag}", Assembly);

                    return imageSource;
                }
                catch
                {
                    return null;
                }
            }
        }

        private string guestToken;

        [JsonProperty("guest_token")]
        public string GuestToken { get => guestToken; set => SetProperty(ref guestToken, value); }

        private bool isVerified;

        [JsonProperty("is_verified")]
        public bool IsVerified { get => isVerified; set => SetProperty(ref isVerified, value, onChanged: () => OnPropertyChanged(nameof(VoteVisible))); }

        private bool isScanned;

        public bool IsScanned { get => isScanned; set => SetProperty(ref isScanned, value, onChanged: () => OnPropertyChanged(nameof(VoteVisible))); }

        public bool VoteVisible => !IsVerified && IsScanned;

        public bool IsValid => !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Email)
            && !string.IsNullOrWhiteSpace(Mobile) && !string.IsNullOrWhiteSpace(PassportId)
            && !string.IsNullOrWhiteSpace(CountryISO) && !string.IsNullOrWhiteSpace(Room)
            && !string.IsNullOrWhiteSpace(PassportImage);

        private bool isSaved;

        public bool IsSaved { get => isSaved; set => SetProperty(ref isSaved, value); }

        private string title;

        public string Title { get => title; set => SetProperty(ref title, value); }
    }
}