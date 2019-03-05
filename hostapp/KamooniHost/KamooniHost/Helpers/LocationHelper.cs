using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;

namespace KamooniHost.Helpers
{
    public class LocationHelper
    {
        public static async Task<bool> IsGeolocationAvailable()
        {
            if (!await PermissionsHelper.CheckPermissions(Permission.Location))
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Application.Current?.MainPage?.DisplayAlert("Location", "Location permission is denied. Please go into Settings and turn on Location for the app.", TranslateExtension.GetValue("ok"));
                });
                return false;
            }

            if (!CrossGeolocator.IsSupported)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Application.Current?.MainPage?.DisplayAlert("Location", "Location is not supported.", TranslateExtension.GetValue("ok"));
                });
                return false;
            }

            if (!CrossGeolocator.Current.IsGeolocationAvailable)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Application.Current?.MainPage?.DisplayAlert("Location", "Location is not available.", TranslateExtension.GetValue("ok"));
                });
                return false;
            }

            return IsGeolocationEnabled();
        }

        public static bool IsGeolocationEnabled()
        {
            if (!CrossGeolocator.Current.IsGeolocationEnabled)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Application.Current?.MainPage?.DisplayAlert("Location", "Location is turn off. Please turn Location on to use the app.", TranslateExtension.GetValue("ok"));
                });
                return false;
            }

            return true;
        }

        public static async Task<Position> GetCurrentPosition(double desiredAccuracy = 100)
        {
            Position position = null;

            try
            {
                if (!await IsGeolocationAvailable())
                    return position;

                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = desiredAccuracy;

                position = await locator.GetLastKnownLocationAsync();

                if (position != null)
                {
                    //got a cahched position, so let's use it.
                    return position;
                }

                position = await locator.GetPositionAsync(TimeSpan.FromSeconds(20), null, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get location: " + ex);
            }

            if (position == null)
                return null;

            var output = string.Format("Time: {0} \nLat: {1} \nLong: {2} \nAltitude: {3} \nAltitude Accuracy: {4} \nAccuracy: {5} \nHeading: {6} \nSpeed: {7}",
                    position.Timestamp, position.Latitude, position.Longitude, position.Altitude, position.AltitudeAccuracy, position.Accuracy, position.Heading, position.Speed);

            Debug.WriteLine(output);

            return position;
        }

        public static async Task<List<Address>> GetAddressesForPosition(Position position)
        {
            List<Address> addresses = null;

            try
            {
                if (!await IsGeolocationAvailable())
                    return addresses;

                var locator = CrossGeolocator.Current;

                addresses = (await locator.GetAddressesForPositionAsync(position))?.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get location: " + ex);
            }

            return addresses ?? new List<Address>();
        }

        public static event EventHandler<PositionEventArgs> PositionChanged;

        public static async Task StartListening()
        {
            if (CrossGeolocator.Current.IsListening)
                return;

            await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(5), 10, true, new ListenerSettings
            {
                ActivityType = ActivityType.AutomotiveNavigation,
                AllowBackgroundUpdates = true,
                DeferLocationUpdates = true,
                DeferralDistanceMeters = 1,
                DeferralTime = TimeSpan.FromSeconds(1),
                ListenForSignificantChanges = true,
                PauseLocationUpdatesAutomatically = false
            });

            CrossGeolocator.Current.PositionChanged += OnPositionChanged;
            CrossGeolocator.Current.PositionError += OnPositionError;
        }

        public static async Task StopListening()
        {
            if (!CrossGeolocator.Current.IsListening)
                return;

            await CrossGeolocator.Current.StopListeningAsync();

            CrossGeolocator.Current.PositionChanged -= OnPositionChanged;
            CrossGeolocator.Current.PositionError -= OnPositionError;
        }

        private static void OnPositionChanged(object sender, PositionEventArgs e)
        {
            //If updating the UI, ensure you invoke on main thread
            var position = e.Position;
            var output = "Full: Lat: " + position.Latitude + " Long: " + position.Longitude;
            output += "\n" + $"Time: {position.Timestamp}";
            output += "\n" + $"Heading: {position.Heading}";
            output += "\n" + $"Speed: {position.Speed}";
            output += "\n" + $"Accuracy: {position.Accuracy}";
            output += "\n" + $"Altitude: {position.Altitude}";
            output += "\n" + $"Altitude Accuracy: {position.AltitudeAccuracy}";
            Debug.WriteLine(output);

            PositionChanged?.Invoke(sender, e);
        }

        private static void OnPositionError(object sender, PositionErrorEventArgs e)
        {
            Debug.WriteLine(e.Error);
            //Handle event here for errors
        }
    }
}