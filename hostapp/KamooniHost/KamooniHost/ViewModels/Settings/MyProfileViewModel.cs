using KamooniHost.Constants;
using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using KamooniHost.Utils;
using KamooniHost.ViewModels.Shared;
using Plugin.Geolocator.Abstractions;
using Stormlion.ImageCropper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels.Settings
{
    public class MyProfileViewModel : TinyViewModel
    {
        private readonly ICountryService countryService;
        private readonly IHostService hostService;

        public Host CurrentHost { get; set; } = Helpers.Settings.CurrentHost;

        private List<State> listState;
        public List<State> ListState { get => listState; set => SetProperty(ref listState, value); }

        private bool provinceSelectorVisible;
        public bool ProvinceSelectorVisible { get => provinceSelectorVisible; set => SetProperty(ref provinceSelectorVisible, value); }

        private bool countrySelectorVisible;
        public bool CountrySelectorVisible { get => countrySelectorVisible; set => SetProperty(ref countrySelectorVisible, value); }
        public ICommand FindOnMapCommand { get; private set; }
        public ICommand SelectProvinceCommand { get; private set; }
        public ICommand CloseProvinceSelectorCommand { get; private set; }
        public ICommand ProvinceSelectedCommand { get; private set; }
        public ICommand SelectCountryCommand { get; private set; }
        public ICommand CountrySelectedCommand { get; private set; }
        public ICommand TakePictureCommand { get; private set; }
        public ICommand UpdateHostCommand { get; private set; }

        public MyProfileViewModel(ICountryService countryService, IHostService hostService)
        {
            this.countryService = countryService;
            this.hostService = hostService;

            FindOnMapCommand = new AwaitCommand(FindOnMap);
            SelectProvinceCommand = new AwaitCommand(SelectProvince);
            CloseProvinceSelectorCommand = new AwaitCommand(CloseProvinceSelector);
            ProvinceSelectedCommand = new AwaitCommand<State>(ProvinceSelected);
            SelectCountryCommand = new AwaitCommand(SelectNationality);
            CountrySelectedCommand = new AwaitCommand<Country>(OnNationalitySelected);
            TakePictureCommand = new AwaitCommand(TakePicture);
            UpdateHostCommand = new AwaitCommand(UpdateHost);
        }

        public override void Init(object data)
        {
            base.Init(data);

            CurrentHost.Country = CountryUtil.GetCountryByISO(CurrentHost.CountryCode);
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            if (CurrentHost.Country != null)
            {
                GetCurrentState();
            }

            MessagingCenter.Subscribe<PickMapViewModel, Address>(this, MessageKey.LOCATION_SAVED, OnLocationPicked);
        }

        public override void OnPopped()
        {
            base.OnPopped();

            MessagingCenter.Unsubscribe<PickMapViewModel, Address>(this, MessageKey.LOCATION_SAVED);
        }

        private async void FindOnMap(object sender, TaskCompletionSource<bool> tcs)
        {
            await CoreMethods.PushViewModel<PickMapViewModel>();

            tcs.SetResult(true);
        }

        private void OnLocationPicked(PickMapViewModel sender, Address address)
        {
            string street = address.SubThoroughfare + ", " + address.Thoroughfare + ", " + address.SubLocality + ", " + address.Locality;

            while (street.Contains(",,") || street.Contains(", ,"))
            {
                street = street.Replace(", ,", ",").Replace(",,", ",");
                if (street.StartsWith(", "))
                    street = street.Remove(0, 2);
                if (street.EndsWith(", "))
                    street = street.Remove(street.Length - 2, 2);
            }

            CurrentHost.Street = street;
            CurrentHost.Street2 = address.SubAdminArea;
            CurrentHost.City = address.AdminArea;
            CurrentHost.Zip = address.PostalCode;
            
            CurrentHost.Country = CountryUtil.GetCountryByISO(address.CountryCode);
            CurrentHost.CountryCode = CurrentHost.Country?.Code;
            CurrentHost.CountryName = CurrentHost.Country?.Name;

            if (CurrentHost.Country != null)
            {
                CurrentHost.StateCode = "";
                CurrentHost.StateName = "";
                GetCurrentState();
            }

            CurrentHost.Latitude = address.Latitude;
            CurrentHost.Longitude = address.Longitude;
        }

        private void GetCurrentState()
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;

            Task.Run(async () =>
            {
                return await countryService.GetStates(CurrentHost.Country.Code);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(async () =>
            {
                IsBusy = false;

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            CurrentHost.State = task.Result.ListState?.Find(st => st.Code == CurrentHost.StateCode);
                        }
                        else if (!string.IsNullOrWhiteSpace(task.Result.Message))
                        {
                            await CoreMethods.DisplayAlert("", task.Result.Message, TranslateExtension.GetValue("ok"));
                        }
                    }
                    else
                    {
                        await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                    }
                }
                else if (task.IsFaulted)
                {
                    await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }));
        }

        private void SelectProvince(object sender, TaskCompletionSource<bool> tcs)
        {
            GetStates();

            tcs.SetResult(true);
        }

        private void GetStates()
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable() || !CountrySelected())
            {
                return;
            }

            IsBusy = true;

            Task.Run(async () =>
            {
                return await countryService.GetStates(CurrentHost.Country.Code);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(async () =>
            {
                IsBusy = false;

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            ListState = task.Result.ListState;

                            ProvinceSelectorVisible = true;
                        }
                        else if (!string.IsNullOrWhiteSpace(task.Result.Message))
                        {
                            await CoreMethods.DisplayAlert("", task.Result.Message, TranslateExtension.GetValue("ok"));
                        }
                    }
                    else
                    {
                        await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                    }
                }
                else if (task.IsFaulted)
                {
                    await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }));
        }

        private void CloseProvinceSelector(object sender, TaskCompletionSource<bool> tcs)
        {
            ProvinceSelectorVisible = false;

            tcs.SetResult(true);
        }

        private void ProvinceSelected(State selected, TaskCompletionSource<bool> tcs)
        {
            CurrentHost.StateCode = selected.Code;
            CurrentHost.StateName = selected.Name;
            CurrentHost.State = selected;
            ProvinceSelectorVisible = false;

            tcs.SetResult(true);
        }

        private void SelectNationality(object sender, TaskCompletionSource<bool> tcs)
        {
            CountrySelectorVisible = true;

            tcs.SetResult(true);
        }

        private void OnNationalitySelected(Country selectedCountry, TaskCompletionSource<bool> tcs)
        {
            CurrentHost.CountryCode = selectedCountry.Code;
            CurrentHost.CountryName = selectedCountry.Name;
            CurrentHost.Country = selectedCountry;

            CountrySelectorVisible = false;

            tcs.SetResult(true);
        }

        private async void TakePicture(object sender, TaskCompletionSource<bool> tcs)
        {
            string imagePath = await PhotoHelper.TakePhotoPathAsync();

            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                try
                {
                    new ImageCropper()
                    {
                        PageTitle = "Avatar",
                        AspectRatioX = 4,
                        AspectRatioY = 3,
                        Success = (imageFile) =>
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                CurrentHost.Image = Convert.ToBase64String(await StorageHelper.LoadImage(imageFile));
                            });
                        }
                    }.Show(CurrentPage, imagePath);
                }
                catch (Exception ex)
                {
                    await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), ex.Message, TranslateExtension.GetValue("ok"));
                }
            }

            tcs.SetResult(true);
        }

        #region Form Validation

        private bool IsFormsValid()
        {
            if (NameEmpty() || EmailEmpty() || MobileEmpty() || !EmailValid() || StreetEmpty() || CityEmpty() || ZipEmpty() || !CountrySelected() || !TypeSelected() || !ImageTaked())
            {
                return false;
            }
            return true;
        }

        private bool NameEmpty()
        {
            if (string.IsNullOrWhiteSpace(CurrentHost.Name))
            {
                CoreMethods.DisplayAlert("Sign Up", "Please enter Property Name", TranslateExtension.GetValue("ok"));
                return true;
            }
            return false;
        }

        private bool EmailEmpty()
        {
            if (string.IsNullOrWhiteSpace(CurrentHost.Email))
            {
                CoreMethods.DisplayAlert("Sign Up", "Please enter Email", TranslateExtension.GetValue("ok"));
                return true;
            }
            return false;
        }

        private bool EmailValid()
        {
            if (!CurrentHost.Email.EmailValidate())
            {
                CoreMethods.DisplayAlert("Sign Up", "Email is invalid", TranslateExtension.GetValue("ok"));
                return false;
            }
            return true;
        }

        private bool MobileEmpty()
        {
            if (string.IsNullOrWhiteSpace(CurrentHost.Mobile))
            {
                CoreMethods.DisplayAlert("Sign Up", "Please enter Mobile Number", TranslateExtension.GetValue("ok"));
                return true;
            }
            return false;
        }

        private bool StreetEmpty()
        {
            if (string.IsNullOrWhiteSpace(CurrentHost.Street))
            {
                CoreMethods.DisplayAlert("Sign Up", "Please enter Street", TranslateExtension.GetValue("ok"));
                return true;
            }
            return false;
        }

        private bool Street2Empty()
        {
            if (string.IsNullOrWhiteSpace(CurrentHost.Street2))
            {
                CoreMethods.DisplayAlert("Sign Up", "Please enter Street 2", TranslateExtension.GetValue("ok"));
                return true;
            }
            return false;
        }

        private bool CityEmpty()
        {
            if (string.IsNullOrWhiteSpace(CurrentHost.City))
            {
                CoreMethods.DisplayAlert("Sign Up", "Please enter City", TranslateExtension.GetValue("ok"));
                return true;
            }
            return false;
        }

        private bool ZipEmpty()
        {
            if (string.IsNullOrWhiteSpace(CurrentHost.Zip))
            {
                CoreMethods.DisplayAlert("Sign Up", "Please enter Postal Code", TranslateExtension.GetValue("ok"));
                return true;
            }
            return false;
        }

        private bool StateSelected()
        {
            if (CurrentHost.State == null)
            {
                CoreMethods.DisplayAlert("Sign Up", "Please select Province", TranslateExtension.GetValue("ok"));
                return false;
            }
            return true;
        }

        private bool CountrySelected()
        {
            if (CurrentHost.Country == null)
            {
                CoreMethods.DisplayAlert("Sign Up", "Please select Country", TranslateExtension.GetValue("ok"));
                return false;
            }
            return true;
        }

        private bool TypeSelected()
        {
            if (!CurrentHost.IsActivity && !CurrentHost.IsAccommodation && !CurrentHost.IsTransport)
            {
                CoreMethods.DisplayAlert("Sign Up", "Please select Type", TranslateExtension.GetValue("ok"));
                return false;
            }
            return true;
        }

        private bool ImageTaked()
        {
            if (string.IsNullOrWhiteSpace(CurrentHost.Image))
            {
                CoreMethods.DisplayAlert("Sign Up", "Please take a Image of Property", TranslateExtension.GetValue("ok"));
                return false;
            }
            return true;
        }

        #endregion Form Validation

        private void UpdateHost(object sender, TaskCompletionSource<bool> tcs)
        {
            if (!IsFormsValid())
            {
                tcs.SetResult(true);
                return;
            }

            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;

            Task.Run(async () =>
            {
                return await hostService.UpdateHost(CurrentHost);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(async () =>
            {
                IsBusy = false;

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            await CoreMethods.DisplayAlert("", "Update host successfully", TranslateExtension.GetValue("ok"));

                            await CoreMethods.PopViewModel();
                        }
                        else if (!string.IsNullOrWhiteSpace(task.Result.Message))
                        {
                            await CoreMethods.DisplayAlert("", task.Result.Message, TranslateExtension.GetValue("ok"));
                        }
                        else
                        {
                            await CoreMethods.DisplayAlert("", "Update host fail", TranslateExtension.GetValue("ok"));
                        }
                    }
                    else
                    {
                        await CoreMethods.DisplayAlert("", TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                    }
                }
                else if (task.IsFaulted)
                {
                    await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }

                tcs.SetResult(true);
            }));
        }
    }
}