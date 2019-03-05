using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels
{
    public class BookingCustomerViewModel : TinyViewModel
    {
        private readonly IBookingsService bookingsService;

        private AddBooking booking = new AddBooking();
        public AddBooking Booking { get => booking; set => SetProperty(ref booking, value); }

        private bool isFormValid;
        public bool IsFormValid { get => isFormValid; set => SetProperty(ref isFormValid, value); }

        public ICommand GoToHomeCommand { get; private set; }
        public ICommand CreateBookingCommand { get; private set; }

        public BookingCustomerViewModel(IBookingsService bookingsService)
        {
            this.bookingsService = bookingsService;

            GoToHomeCommand = new AwaitCommand(GoToHome);
            CreateBookingCommand = new AwaitCommand(CreateBooking);
        }

        public override void Init(object data)
        {
            base.Init(data);

            Booking = ((AddBooking)data)?.DeepCopy();

            if (Booking != null)
            {
                Booking.Customer.PropertyChanged += Customer_PropertyChanged;
            }
        }

        private void CheckFormValid(Customer customer)
        {
            IsFormValid = !string.IsNullOrWhiteSpace(customer.Name) && !string.IsNullOrWhiteSpace(customer.Email) && !string.IsNullOrWhiteSpace(customer.Mobile);
        }

        private void Customer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CheckFormValid((Customer)sender);
        }

        private void GoToHome(object sender, TaskCompletionSource<bool> tcs)
        {
            App.Current.MainPage = new MainPage();
            tcs.SetResult(true);
        }

        private void CreateBooking(object sender, TaskCompletionSource<bool> tcs)
        {
            if (!StringExtensions.EmailValidate(Booking.Customer.Email))
            {
                CoreMethods.DisplayAlert("", "Email is invalid.", TranslateExtension.GetValue("ok"));
                tcs.SetResult(true);
                return;
            }

            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                tcs.SetResult(true);
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await bookingsService.CreateBooking(Booking);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(async () =>
            {
                IsBusy = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            await CoreMethods.DisplayAlert("Booking", "Booking Successful", TranslateExtension.GetValue("ok"));

                            if (Application.Current.MainPage is MasterDetailPage masterDetailPage)
                            {
                                if (masterDetailPage.Detail is INavigationService detailPage1)
                                {
                                    detailPage1.NotifyChildrenPageWasPopped();
                                }
                                else if (masterDetailPage.Detail is NavigationPage detailPage2)
                                {
                                    detailPage2.NotifyAllChildrenPopped();
                                }
                            }
                            Application.Current.MainPage = new MainPage();
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(task.Result?.Message))
                                await CoreMethods.DisplayAlert("Booking", task.Result.Message, TranslateExtension.GetValue("ok"));
                            else
                                await CoreMethods.DisplayAlert("Booking", "Booking Failed", TranslateExtension.GetValue("ok"));
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
            }));

            tcs.SetResult(true);
        }
    }
}