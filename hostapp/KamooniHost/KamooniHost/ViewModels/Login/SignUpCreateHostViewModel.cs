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

namespace KamooniHost.ViewModels
{
    public class SignUpCreateHostViewModel : TinyViewModel
    {
        private readonly ICountryService countryService;
        private readonly ISignUpService signUpService;

        public HostToCreate HostToCreate { get; set; } = new HostToCreate();

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
        public ICommand CreateHostCommand { get; private set; }

        public SignUpCreateHostViewModel(ICountryService countryService, ISignUpService signUpService)
        {
            this.countryService = countryService;
            this.signUpService = signUpService;

            FindOnMapCommand = new AwaitCommand(FindOnMap);
            SelectProvinceCommand = new AwaitCommand(SelectProvince);
            CloseProvinceSelectorCommand = new AwaitCommand(CloseProvinceSelector);
            ProvinceSelectedCommand = new AwaitCommand<State>(ProvinceSelected);
            SelectCountryCommand = new AwaitCommand(SelectNationality);
            CountrySelectedCommand = new AwaitCommand<Country>(OnNationalitySelected);
            TakePictureCommand = new AwaitCommand(TakePicture);
            CreateHostCommand = new AwaitCommand(CreateHost);
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            HostToCreate.Country = CountryUtil.GetCountryByISO("ZA");

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

            HostToCreate.Street = street;
            HostToCreate.Street2 = address.SubAdminArea;
            HostToCreate.City = address.AdminArea;
            HostToCreate.Zip = address.PostalCode;

            HostToCreate.Country = CountryUtil.GetCountryByISO(address.CountryCode);

            if (HostToCreate.Country != null)
            {
                HostToCreate.State = new State();
            }

            HostToCreate.Latitude = address.Latitude;
            HostToCreate.Longitude = address.Longitude;
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
                return await countryService.GetStates(HostToCreate.Country.Code);
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
                            await CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_sign_up"), task.Result.Message, TranslateExtension.GetValue("ok"));
                        }
                        else
                        {
                            await CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_sign_up"), TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                        }
                    }
                    else
                    {
                        await CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_sign_up"), TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
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
            HostToCreate.State = selected;
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
            HostToCreate.Country = selectedCountry;
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
                                HostToCreate.Image = Convert.ToBase64String(await StorageHelper.LoadImage(imageFile));
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
            if (string.IsNullOrWhiteSpace(HostToCreate.Name))
            {
                CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_sign_up"), "Please enter Property Name", TranslateExtension.GetValue("ok"));
                return true;
            }
            return false;
        }

        private bool EmailEmpty()
        {
            if (string.IsNullOrWhiteSpace(HostToCreate.Email))
            {
                CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_sign_up"), "Please enter Email", TranslateExtension.GetValue("ok"));
                return true;
            }
            return false;
        }

        private bool EmailValid()
        {
            if (!HostToCreate.Email.EmailValidate())
            {
                CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_sign_up"), "Email is invalid", TranslateExtension.GetValue("ok"));
                return false;
            }
            return true;
        }

        private bool MobileEmpty()
        {
            if (string.IsNullOrWhiteSpace(HostToCreate.Mobile))
            {
                CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_sign_up"), "Please enter Mobile Number", TranslateExtension.GetValue("ok"));
                return true;
            }
            return false;
        }

        private bool StreetEmpty()
        {
            if (string.IsNullOrWhiteSpace(HostToCreate.Street))
            {
                CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_sign_up"), "Please enter Street", TranslateExtension.GetValue("ok"));
                return true;
            }
            return false;
        }

        private bool Street2Empty()
        {
            if (string.IsNullOrWhiteSpace(HostToCreate.Street2))
            {
                CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_sign_up"), "Please enter Street 2", TranslateExtension.GetValue("ok"));
                return true;
            }
            return false;
        }

        private bool CityEmpty()
        {
            if (string.IsNullOrWhiteSpace(HostToCreate.City))
            {
                CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_sign_up"), "Please enter City", TranslateExtension.GetValue("ok"));
                return true;
            }
            return false;
        }

        private bool ZipEmpty()
        {
            if (string.IsNullOrWhiteSpace(HostToCreate.Zip))
            {
                CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_sign_up"), "Please enter Postal Code", TranslateExtension.GetValue("ok"));
                return true;
            }
            return false;
        }

        private bool StateSelected()
        {
            if (HostToCreate.State == null)
            {
                CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_sign_up"), "Please select Province", TranslateExtension.GetValue("ok"));
                return false;
            }
            return true;
        }

        private bool CountrySelected()
        {
            if (HostToCreate.Country == null)
            {
                CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_sign_up"), "Please select Country", TranslateExtension.GetValue("ok"));
                return false;
            }
            return true;
        }

        private bool TypeSelected()
        {
            if (!HostToCreate.IsActivity && !HostToCreate.IsAccommodation && !HostToCreate.IsTransport)
            {
                CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_sign_up"), "Please select Type", TranslateExtension.GetValue("ok"));
                return false;
            }
            return true;
        }

        private bool ImageTaked()
        {
            if (string.IsNullOrWhiteSpace(HostToCreate.Image))
            {
                CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_sign_up"), "Please take a Image of Property", TranslateExtension.GetValue("ok"));
                return false;
            }
            return true;
        }

        #endregion Form Validation

        private void CreateHost(object sender, TaskCompletionSource<bool> tcs)
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
                return await signUpService.CreateHost(HostToCreate);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(async () =>
            {
                IsBusy = false;

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            await CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_sign_up"), TranslateExtension.GetValue("dialog_message_host_selected"), "GOT IT");

                            Application.Current.MainPage = new NavigationContainer(ViewModelResolver.ResolveViewModel<LoginViewModel>())
                            {
                                BarBackgroundColor = Color.FromHex("#835e7e"),
                                BarTextColor = Color.White
                            };
                        }
                        else if (!string.IsNullOrWhiteSpace(task.Result.Message))
                        {
                            await CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_sign_up"), task.Result.Message, TranslateExtension.GetValue("ok"));
                        }
                        else
                        {
                            await CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_sign_up"), TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                        }
                    }
                    else
                    {
                        await CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_sign_up"), TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
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