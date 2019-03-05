using Acr.UserDialogs;
using KamooniHost.Constants;
using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels
{
    public class PayNowViewModel : TinyViewModel
    {
        private readonly IBookingsService openBookingsService;

        private Booking booking;

        private bool needToRefresh;

        private ObservableCollection<GuestBill> listGuestBills = new ObservableCollection<GuestBill>();
        public ObservableCollection<GuestBill> ListGuestBills { get => listGuestBills; set => SetProperty(ref listGuestBills, value); }

        private bool payAllVisible;
        public bool PayAllVisible { get => payAllVisible; set => SetProperty(ref payAllVisible, value); }

        private GuestBill selectedGuest;
        public GuestBill SelectedGuest { get => selectedGuest; set => SetProperty(ref selectedGuest, value); }

        private List<PaymentMethod> listPaymentMethod = new List<PaymentMethod>();
        public List<PaymentMethod> ListPaymentMethod { get => listPaymentMethod; set => SetProperty(ref listPaymentMethod, value); }

        private bool selectPayerVisible;
        public bool SelectPayerVisible { get => selectPayerVisible; set => SetProperty(ref selectPayerVisible, value); }

        private PaymentMethod paymentMethod;
        public PaymentMethod PaymentMethod { get => paymentMethod; set => SetProperty(ref paymentMethod, value, onChanged: OnPaymentMethodChanged); }

        private double payBalance;
        public double PayBalance { get => payBalance; set => SetProperty(ref payBalance, value); }

        private double paymentAmount;
        public double PaymentAmount { get => paymentAmount; set => SetProperty(ref paymentAmount, value, onChanged: OnPaymentAmountChanged); }

        private bool isPayAll;
        public bool IsPayAll { get => isPayAll; set => SetProperty(ref isPayAll, value, onChanged: OnIsPayAllChanged); }

        private bool paymentVisible;
        public bool PaymentVisible { get => paymentVisible; set => SetProperty(ref paymentVisible, value); }

        private bool confirmPaymentVisible;
        public bool ConfirmPaymentVisible { get => confirmPaymentVisible; set => SetProperty(ref confirmPaymentVisible, value); }

        private bool payAndCheckOutVisible;
        public bool PayAndCheckOutVisible { get => payAndCheckOutVisible; set => SetProperty(ref payAndCheckOutVisible, value); }

        private bool sendPaymentLinkVisible;
        public bool SendPaymentLinkVisible { get => sendPaymentLinkVisible; set => SetProperty(ref sendPaymentLinkVisible, value); }

        private bool paymentExecuting;
        public bool PaymentExecuting { get => paymentExecuting; set => SetProperty(ref paymentExecuting, value); }

        private bool isConfirmPIN;
        public bool IsConfirmPIN { get => isConfirmPIN; set => SetProperty(ref isConfirmPIN, value); }


        public ICommand PayNowCommand { get; private set; }
        public ICommand PayAllCommand { get; private set; }
        public ICommand PayerSelectedCommand { get; private set; }
        public ICommand ClosePayerSelectCommand { get; private set; }
        public ICommand ConfirmPaymentCommand { get; private set; }
        public ICommand PayAndCheckOutCommand { get; private set; }
        public ICommand SendPaymentLinkCommand { get; private set; }
        public ICommand ClosePayNowCommand { get; private set; }

        public PayNowViewModel(IBookingsService openBookingsService)
        {
            this.openBookingsService = openBookingsService;

            PayNowCommand = new AwaitCommand<GuestBill>(PayNow);
            PayAllCommand = new AwaitCommand(PayAll);
            PayerSelectedCommand = new AwaitCommand<GuestBill>(PayerSelected);
            ClosePayerSelectCommand = new AwaitCommand(ClosePayerSelect);
            ConfirmPaymentCommand = new AwaitCommand(ConfirmPayment);
            PayAndCheckOutCommand = new AwaitCommand(PayAndCheckOut);
            SendPaymentLinkCommand = new AwaitCommand(SendPaymentLink);
            ClosePayNowCommand = new AwaitCommand(ClosePayNow);
        }

        public override void Init(object data)
        {
            base.Init(data);

            booking = (Booking)data;
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            GetBills();
            GetListPaymentMethod();
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            if (needToRefresh)
            {
                GetBills();

                needToRefresh = false;

                MessagingCenter.Send(this, MessageKey.BILL_CHANGED);
            }
        }

        private void GetBills()
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await openBookingsService.GetBills(booking.Id);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    ListGuestBills.Clear();

                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            foreach (var guest in task.Result.GuestBills)
                            {
                                foreach (var item in guest.BillItems?.GroupBy(b => b.Category))
                                {
                                    guest.BillItemsByGroup.Add(new BillItemsByGroup()
                                    {
                                        Key = item.Key,
                                        ItemsByGroup = new ObservableCollection<BillItem>(item.OrderBy(b => b.Product).ToList()),
                                        Total = item.Sum(b => b.NetPrice)
                                    });
                                }
                            }
                            ListGuestBills = new ObservableCollection<GuestBill>(task.Result.GuestBills);

                            PayAllVisible = ListGuestBills.Sum(b => b.GuestDetails.Balance) > 0;
                        }
                        else if (!string.IsNullOrWhiteSpace(task.Result.Message))
                        {
                            CoreMethods.DisplayAlert("", task.Result.Message, TranslateExtension.GetValue("ok"));
                        }
                    }
                    else
                    {
                        CoreMethods.DisplayAlert("", TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                    }
                }
                else if (task.IsFaulted)
                {
                    CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }));
        }

        private void GetListPaymentMethod()
        {
            if (!ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            Task.Run(async () =>
            {
                return await openBookingsService.GetPaymentMethods();
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            ListPaymentMethod = task.Result.ListPaymentMethod;
                        }
                        else if (!string.IsNullOrWhiteSpace(task.Result.Message))
                        {
                            CoreMethods.DisplayAlert("", task.Result.Message, TranslateExtension.GetValue("ok"));
                        }
                    }
                    else
                    {
                        CoreMethods.DisplayAlert("", TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                    }
                }
                else if (task.IsFaulted)
                {
                    CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }));
        }

        private void OnIsPayAllChanged()
        {
            if (IsPayAll)
            {
                ConfirmPaymentVisible = Math.Round(PaymentAmount, 2) > 0;
                PayAndCheckOutVisible = Math.Round(PaymentAmount, 2) >= PayBalance;
            }
            else
            {
                ConfirmPaymentVisible = Math.Round(PaymentAmount, 2) > 0;
                PayAndCheckOutVisible = false;
            }
        }

        private async void PayNow(GuestBill sender, TaskCompletionSource<bool> tcs)
        {
            if (!string.IsNullOrEmpty(Helpers.Settings.PIN))
            {
                PromptConfig t_config = new PromptConfig();
                t_config.SetCancelText("No");
                t_config.SetOkText("Yes");
                t_config.SetInputMode(InputType.Password);
                t_config.SetText("");
                t_config.SetPlaceholder("Input your PIN");

                PromptResult tm = await UserDialogs.Instance.PromptAsync(t_config);

                if (tm.Ok)
                {
                    if (tm.Text.Equals(Helpers.Settings.PIN)) { 
                        PaymentVisible = true;

                        SelectedGuest = sender;

                        PayBalance = sender.GuestDetails.Balance;
                        PaymentMethod = ListPaymentMethod.FirstOrDefault();
                        PaymentAmount = sender.GuestDetails.Balance;
                        IsPayAll = false;
                    }
                    else
                    {
                        await CoreMethods.DisplayAlert("", "You just inputed invalid PIN", "OK");
                    }
                }
                tcs.SetResult(true);
            }
            else { 
                PaymentVisible = true;

                SelectedGuest = sender;

                PayBalance = sender.GuestDetails.Balance;
                PaymentMethod = ListPaymentMethod.FirstOrDefault();
                PaymentAmount = sender.GuestDetails.Balance;
                IsPayAll = false;

                tcs.SetResult(true);
            }
        }

      

        private async void PayAll(object sender, TaskCompletionSource<bool> tcs)
        {
            if (!string.IsNullOrEmpty(Helpers.Settings.PIN))
            {
                PromptConfig t_config = new PromptConfig();
                t_config.SetCancelText("No");
                t_config.SetOkText("Yes");
                t_config.SetInputMode(InputType.Password);
                t_config.SetText("");
                t_config.SetPlaceholder("Input your PIN");

                PromptResult tm = await UserDialogs.Instance.PromptAsync(t_config);

                if (tm.Ok)
                {
                    if (tm.Text.Equals(Helpers.Settings.PIN))
                    {
                        if (ListGuestBills.Count == 1)
                        {
                            PaymentVisible = true;

                            SelectedGuest = ListGuestBills.FirstOrDefault();

                            PayBalance = ListGuestBills.Sum(g => g.GuestDetails.Balance);
                            PaymentMethod = ListPaymentMethod.FirstOrDefault();
                            PaymentAmount = ListGuestBills.Sum(g => g.GuestDetails.Balance);
                            IsPayAll = true;

                            CheckPaymentCondition();
                        }
                        else
                        {
                            SelectPayerVisible = true;
                        }
                    }
                    else
                    {
                        await CoreMethods.DisplayAlert("", "You just inputed invalid PIN", "OK");
                    }
                }
            }else { 

                if (ListGuestBills.Count == 1)
                {
                    PaymentVisible = true;

                    SelectedGuest = ListGuestBills.FirstOrDefault();

                    PayBalance = ListGuestBills.Sum(g => g.GuestDetails.Balance);
                    PaymentMethod = ListPaymentMethod.FirstOrDefault();
                    PaymentAmount = ListGuestBills.Sum(g => g.GuestDetails.Balance);
                    IsPayAll = true;

                    CheckPaymentCondition();
                }
                else
                {
                    SelectPayerVisible = true;
                }
            }
            tcs.SetResult(true);
        }

        private void PayerSelected(GuestBill sender, TaskCompletionSource<bool> tcs)
        {
            SelectPayerVisible = false;

            PaymentVisible = true;

            SelectedGuest = sender;

            PayBalance = ListGuestBills.Sum(g => g.GuestDetails.Balance);
            PaymentMethod = ListPaymentMethod.FirstOrDefault();
            PaymentAmount = ListGuestBills.Sum(g => g.GuestDetails.Balance);
            IsPayAll = true;

            CheckPaymentCondition();

            tcs.SetResult(true);
        }

        private void ClosePayerSelect(object sender, TaskCompletionSource<bool> tcs)
        {
            SelectPayerVisible = false;

            tcs.SetResult(true);
        }

        private void OnPaymentMethodChanged()
        {
            CheckPaymentCondition();
        }

        private void OnPaymentAmountChanged()
        {
            CheckPaymentCondition();
        }

        private void CheckPaymentCondition()
        {
            if (PaymentMethod != null && PaymentMethod.Id.Equals(99))
            {
                ConfirmPaymentVisible = false;
                PayAndCheckOutVisible = false;
                //SendPaymentLinkVisible = Math.Round(PaymentAmount, 2) >= PayBalance;
                SendPaymentLinkVisible = Math.Round(PaymentAmount, 2) > 0;
            }
            else
            {
                if (IsPayAll)
                {
                    ConfirmPaymentVisible = Math.Round(PaymentAmount, 2) > 0;
                    PayAndCheckOutVisible = Math.Round(PaymentAmount, 2) >= PayBalance;
                }
                else
                {
                    ConfirmPaymentVisible = Math.Round(PaymentAmount, 2) > 0;
                    PayAndCheckOutVisible = false;
                }
                SendPaymentLinkVisible = false;
            }
        }

        private void ConfirmPayment(object sender, TaskCompletionSource<bool> tcs)
        {
            PostPayment();
            tcs.SetResult(true);
        }

        private void PayAndCheckOut(object sender, TaskCompletionSource<bool> tcs)
        {
            PostPayment(true);
            tcs.SetResult(true);
        }

        private void SendPaymentLink(object sender, TaskCompletionSource<bool> tcs)
        {
            if (!ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            UserDialogs.Instance.Loading("Sending Payment Link...").Show();

            Task.Run(async () =>
            {
                return await openBookingsService.SendPaymentLink(booking.Id, SelectedGuest.GuestDetails.Id, Math.Round(PaymentAmount, 2));
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                UserDialogs.Instance.Loading().Hide();

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            needToRefresh = true;
                            PaymentVisible = false;

                            Device.OpenUri(new Uri(task.Result.PaymentLink));
                        }
                        else if (!string.IsNullOrWhiteSpace(task.Result.Message))
                        {
                            UserDialogs.Instance.Toast(task.Result.Message, TimeSpan.FromSeconds(5));
                        }
                    }
                    else
                    {
                        UserDialogs.Instance.Toast(TranslateExtension.GetValue("dialog_message_server_error"), TimeSpan.FromSeconds(5));
                    }
                }
                else if (task.IsFaulted)
                {
                    CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }

                tcs.TrySetResult(true);
            }));
        }

        private void ClosePayNow(object sender, TaskCompletionSource<bool> tcs)
        {
            SelectedGuest = null;
            IsPayAll = false;
            PaymentVisible = false;

            tcs.SetResult(true);
        }

        private void PostPayment(bool checkoutQueued = false)
        {
            if (PaymentMethod == null)
            {
                CoreMethods.DisplayAlert("", "Payment is not selected", TranslateExtension.GetValue("ok"));
                return;
            }

            if (PaymentAmount <= 0)
            {
                CoreMethods.DisplayAlert("", "Payment amount must be greater than 0", TranslateExtension.GetValue("ok"));
                return;
            }

            if (PaymentExecuting || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            PaymentExecuting = true;
            Task.Run(async () =>
            {
                return await openBookingsService.PostPayment(booking.Id, SelectedGuest.GuestDetails.Id, PaymentMethod.Id, Math.Round(PaymentAmount, 2));
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                PaymentExecuting = false;

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            if (checkoutQueued)
                            {
                                CheckOut();
                            }
                            else
                            {
                                CoreMethods.DisplayAlert("", "Payment Registered", TranslateExtension.GetValue("ok"));

                                GetBills();

                                MessagingCenter.Send(this, MessageKey.BILL_CHANGED);

                                PaymentVisible = false;
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(task.Result.Message))
                        {
                            CoreMethods.DisplayAlert("", task.Result.Message, TranslateExtension.GetValue("ok"));
                        }
                    }
                    else
                    {
                        CoreMethods.DisplayAlert("", TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                    }
                }
                else if (task.IsFaulted)
                {
                    CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }));
        }

        private void CheckOut()
        {
            if (PaymentExecuting || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            PaymentExecuting = true;
            Task.Run(async () =>
            {
                return await openBookingsService.Checkout(booking.Id);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(async () =>
            {
                PaymentExecuting = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            await CoreMethods.DisplayAlert("", "Checkout Successful", TranslateExtension.GetValue("ok"));

                            MessagingCenter.Send(this, MessageKey.BILL_CHANGED);

                            await CoreMethods.PopViewModel();
                        }
                        else if (!string.IsNullOrWhiteSpace(task.Result.Message))
                        {
                            await CoreMethods.DisplayAlert("", task.Result.Message, TranslateExtension.GetValue("ok"));
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
        }
    }
}