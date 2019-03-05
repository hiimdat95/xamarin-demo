using System.Reflection;
using Xamarin.Forms;

namespace KamooniHost.Models
{
    public class Country : BaseModel
    {
        private string code;

        public string Code { get => code; set => SetProperty(ref code, value); }

        private string name;

        public string Name { get => name; set => SetProperty(ref name, value); }

        private string dialCode;

        public string DialCode { get => dialCode; set => SetProperty(ref dialCode, value); }

        private string flag;

        public string Flag { get => flag; set => SetProperty(ref flag, value, onChanged: () => OnPropertyChanged(nameof(FlagImage))); }

        public ImageSource FlagImage
        {
            get
            {
                try
                {
                    if (Flag == null)
                        return null;

                    var Assembly = typeof(App).GetTypeInfo().Assembly;

                    var strAssemblyName = Assembly.GetName().Name;
                    var resource = "AppResources.Assets";
                    var imageSource = ImageSource.FromResource($"{strAssemblyName}.{resource}.{Flag}", Assembly);

                    return imageSource;
                }
                catch
                {
                    return null;
                }
            }
        }

        public Country()
        {
        }

        public Country(string code, string name, string dialCode, string flag)
        {
            Code = code;
            Name = name;
            DialCode = dialCode;
            Flag = flag;
        }
    }
}