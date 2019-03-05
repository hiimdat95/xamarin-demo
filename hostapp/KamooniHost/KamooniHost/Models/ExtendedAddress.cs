using Plugin.Geolocator.Abstractions;

namespace KamooniHost.Models
{
    public class ExtendedAddress : Address
    {
        public string FullAddress
        {
            get
            {
                var result = "";
                //if (!string.IsNullOrWhiteSpace(FeatureName))
                //    result += FeatureName + ", ";
                if (!string.IsNullOrWhiteSpace(SubThoroughfare) && SubThoroughfare != Thoroughfare)
                    result += SubThoroughfare + ", ";
                if (!string.IsNullOrWhiteSpace(Thoroughfare))
                    result += Thoroughfare + ", ";
                if (!string.IsNullOrWhiteSpace(SubLocality) && SubLocality != Locality)
                    result += SubLocality + ", ";
                if (!string.IsNullOrWhiteSpace(Locality))
                    result += Locality + ", ";
                if (!string.IsNullOrWhiteSpace(SubAdminArea) && SubAdminArea != AdminArea)
                    result += SubAdminArea + ", ";
                if (!string.IsNullOrWhiteSpace(AdminArea))
                    result += AdminArea;
                if (result.EndsWith(","))
                    result = result.Remove(result.Length - 1, 1);
                return result?.Trim();
            }
        }
    }
}