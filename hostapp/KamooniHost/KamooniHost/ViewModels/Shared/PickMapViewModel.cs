using KamooniHost.Constants;
using KamooniHost.Helpers;
using KamooniHost.Models;
using KamooniHost.Models.Local;
using Newtonsoft.Json;
using Plugin.Geolocator.Abstractions;
using SQLite;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels.Shared
{
    public class PickMapViewModel : TinyViewModel
    {
        private readonly SQLiteConnection localDb;

        private Map map;

        private Address address;

        private ObservableCollection<ExtendedAddress> addresses = new ObservableCollection<ExtendedAddress>();
        public ObservableCollection<ExtendedAddress> Addresses { get => addresses; set => SetProperty(ref addresses, value); }

        private bool addressSelectorVisible;
        public bool AddressSelectorVisible { get => addressSelectorVisible; set => SetProperty(ref addressSelectorVisible, value); }

        public ICommand SaveLocationCommand { get; set; }
        public ICommand AddressSelectedCommand { get; set; }

        public PickMapViewModel(SQLiteConnection localDb)
        {
            this.localDb = localDb;

            SaveLocationCommand = new AwaitCommand(SaveLocation);
            AddressSelectedCommand = new AwaitCommand<ExtendedAddress>(AddressSelected);
        }

        public override void Init(object data)
        {
            base.Init(data);

            map = CurrentPage.FindByName<Map>("map");
        }

        public async override void OnPageCreated()
        {
            base.OnPageCreated();

            if (await LocationHelper.IsGeolocationAvailable())
            {
                map.MyLocationEnabled = true;
                map.UiSettings.MyLocationButtonEnabled = true;

                map.MyLocationButtonClicked += Map_MyLocationButtonClicked;
                map.MapClicked += Map_MapClicked;

                var userData = localDb.Table<UserData>().FirstOrDefault(u => u.Name == "LastUserLocation");
                if (userData != null)
                {
                    var lastUserLocation = JsonConvert.DeserializeObject<Plugin.Geolocator.Abstractions.Position>(userData.Value);
                    if (lastUserLocation != null)
                    {
                        map.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.GoogleMaps.Position(lastUserLocation.Latitude, lastUserLocation.Longitude), Distance.FromMeters(1000)));
                    }
                }
            }
        }

        private async void Map_MyLocationButtonClicked(object sender, MyLocationButtonClickedEventArgs e)
        {
            var location = await LocationHelper.GetCurrentPosition();

            if (location != null)
            {
                localDb.InsertOrReplace(new UserData
                {
                    Name = "LastUserLocation",
                    Value = JsonConvert.SerializeObject(location)
                });
            }
        }

        private async void Map_MapClicked(object sender, MapClickedEventArgs e)
        {
            AddressSelectorVisible = true;

            var results = await LocationHelper.GetAddressesForPosition(new Plugin.Geolocator.Abstractions.Position(e.Point.Latitude, e.Point.Longitude));

            Addresses.Clear();

            foreach (var address in results)
            {
                Addresses.Add(new ExtendedAddress
                {
                    AdminArea = address.AdminArea,
                    SubAdminArea = address.SubAdminArea,
                    Locality = address.Locality,
                    SubLocality = address.SubLocality,
                    Thoroughfare = address.Thoroughfare,
                    SubThoroughfare = address.SubThoroughfare,
                    FeatureName = address.FeatureName,
                    CountryCode = address.CountryCode,
                    CountryName = address.CountryName,
                    PostalCode = address.PostalCode,
                    Latitude = address.Latitude,
                    Longitude = address.Longitude
                });
            }
        }

        private void AddressSelected(ExtendedAddress sender, TaskCompletionSource<bool> tcs)
        {
            map.SelectedPin = null;
            map.Pins.Clear();
            map.Pins.Add(new Pin
            {
                Type = PinType.Place,
                Label = "Your are here",
                Address = sender.FullAddress,
                Position = new Xamarin.Forms.GoogleMaps.Position(sender.Latitude, sender.Longitude),
                IsDraggable = false
            });

            map.SelectedPin = map.Pins.FirstOrDefault();

            address = sender;

            tcs.SetResult(true);
        }

        private async void SaveLocation(object sender, TaskCompletionSource<bool> tcs)
        {
            if (address != null)
            {
                MessagingCenter.Send(this, MessageKey.LOCATION_SAVED, address);
            }

            await CoreMethods.PopViewModel();

            tcs.SetResult(true);
        }
    }
}