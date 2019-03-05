using Newtonsoft.Json;
using System.Reflection;
using Xamarin.Forms;

namespace KamooniHost.Models
{
    public class GuestCheckIn : BaseModel
    {
        private string name;

        [JsonProperty("name")]
        public string Name { get => name; set => SetProperty(ref name, value); }

        private string mobile;

        [JsonProperty("mobile")]
        public string Mobile { get => mobile; set => SetProperty(ref mobile, value); }

        private string email;

        [JsonProperty("email")]
        public string Email { get => email; set => SetProperty(ref email, value); }

        private string country;

        [JsonProperty("country")]
        public string Country { get => country; set => SetProperty(ref country, value); }

        private string countryName;
        
        public string CountryName { get => countryName; set => SetProperty(ref countryName, value); }

        private string countryFlag;
        
        public string CountryFlag { get => countryFlag; set => SetProperty(ref countryFlag, value, onChanged: () => OnPropertyChanged(nameof(CountryFlagImage))); }

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

        private string passport;

        [JsonProperty("passport")]
        public string Passport { get => passport; set => SetProperty(ref passport, value); }

        private string passportImage;

        [JsonProperty("passport_image")]
        public string PassportImage { get => passportImage; set => SetProperty(ref passportImage, value); }
    }
}