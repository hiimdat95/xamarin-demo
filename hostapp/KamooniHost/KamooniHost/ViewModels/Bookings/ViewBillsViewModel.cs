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
using Xamarin.Forms.Controls;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels
{
    public class ViewBillsViewModel : TinyViewModel
    {
        private readonly IBookingsService openBookingsService;

        private Booking booking;

        private GuestBill selectedGuest;

        private List<GuestBill> listGuest = new List<GuestBill>();

        private bool isBusy, needToRefresh;

        private ObservableCollection<TabbedItem> listTabbedItem = new ObservableCollection<TabbedItem>();
        public ObservableCollection<TabbedItem> ListTabbedItem { get => listTabbedItem; set => SetProperty(ref listTabbedItem, value); }

        private List<PaymentMethod> listPaymentMethod = new List<PaymentMethod>();
        public List<PaymentMethod> ListPaymentMethod { get => listPaymentMethod; set => SetProperty(ref listPaymentMethod, value); }

        private PaymentMethod paymentMethod;
        public PaymentMethod PaymentMethod { get => paymentMethod; set => SetProperty(ref paymentMethod, value, onChanged: OnPaymentMethodChanged); }

        private double paymentAmount;
        public double PaymentAmount { get => paymentAmount; set => SetProperty(ref paymentAmount, value, onChanged: PaymentAmountChanged); }

        private bool canConfirmPayment;
        public bool CanConfirmPayment { get => canConfirmPayment; set => SetProperty(ref canConfirmPayment, value); }

        private bool canPayAndCheckOut;
        public bool CanPayAndCheckOut { get => canPayAndCheckOut; set => SetProperty(ref canPayAndCheckOut, value); }

        private bool sendPaymentLinkVisible;
        public bool SendPaymentLinkVisible { get => sendPaymentLinkVisible; set => SetProperty(ref sendPaymentLinkVisible, value); }

        private bool paymentVisible;
        public bool PaymentVisible { get => paymentVisible; set => SetProperty(ref paymentVisible, value); }

        private bool paymentExecuting;
        public bool PaymentExecuting { get => paymentExecuting; set => SetProperty(ref paymentExecuting, value); }

        public ICommand AddExtrasCommand { get; private set; }
        public ICommand PayNowCommand { get; private set; }
        public ICommand ConfirmPaymentCommand { get; private set; }
        public ICommand PayAndCheckOutCommand { get; private set; }
        public ICommand SendPaymentLinkCommand { get; private set; }
        public ICommand ClosePayNowCommand { get; private set; }

        public ViewBillsViewModel(IBookingsService openBookingsService)
        {
            this.openBookingsService = openBookingsService;

            AddExtrasCommand = new AwaitCommand<GuestBill>(AddExtras);
            PayNowCommand = new AwaitCommand<GuestBill>(PayNow);
            ConfirmPaymentCommand = new AwaitCommand<GuestBill>(ConfirmPayment);
            PayAndCheckOutCommand = new AwaitCommand<GuestBill>(PayAndCheckOut);
            SendPaymentLinkCommand = new AwaitCommand<GuestBill>(SendPaymentLink);
            ClosePayNowCommand = new AwaitCommand<GuestBill>(ClosePayNow);
        }

        public override void Init(object data)
        {
            base.Init(data);

            booking = (Booking)data;
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            MessagingCenter.Subscribe<ExtraSummaryViewModel>(this, MessageKey.EXTRA_ITEMS_ADDED, OnItemAdded);

            if (CurrentPage is TabbedPage page)
            {
                page.CurrentPageChanged += Page_CurrentPageChanged;
            }

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

        public override void OnPopped()
        {
            base.OnPopped();

            MessagingCenter.Unsubscribe<ExtraSummaryViewModel>(this, MessageKey.EXTRA_ITEMS_ADDED);
        }

        private void OnItemAdded(ExtraSummaryViewModel sender)
        {
            GetBills();
            MessagingCenter.Send(this, MessageKey.BILL_CHANGED);
        }

        private void Page_CurrentPageChanged(object sender, EventArgs e)
        {
            PaymentVisible = false;
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
                    listGuest.Clear();
                    ListTabbedItem.Clear();

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

                                if (guest.BillItemsByGroup.Count == 1)
                                {
                                    guest.BillItemsByGroup.First().IsExpanded = true;
                                }

                                listGuest.Add(guest);
                                ListTabbedItem.Add(new TabbedItem()
                                {
                                    HeaderText = guest.GuestDetails.Name,
                                    BindingContext = guest
                                });

                                guest.GuestDetails.PropertyChanged += (s, e) =>
                                {
                                    if (e.PropertyName == nameof(guest.GuestDetails.Paid) || e.PropertyName == nameof(guest.GuestDetails.Amount))
                                    {
                                        guest.OnPropertyChanged(nameof(guest.CanPayment));
                                    }
                                };

                                foreach (var item in guest.BillItemsByGroup)
                                {
                                    item.PropertyChanged += (s, e) =>
                                    {
                                        if (isBusy)
                                            return;
                                        if (e.PropertyName == nameof(item.IsExpanded))
                                        {
                                            isBusy = true;
                                            foreach (var other in guest.BillItemsByGroup)
                                            {
                                                if (other != s)
                                                {
                                                    other.IsExpanded = false;
                                                }
                                            }
                                            isBusy = false;
                                        }
                                    };
                                }
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

        private async void AddExtras(GuestBill sender, TaskCompletionSource<bool> tcs)
        {
            var @params = new NavigationParameters()
            {
                { ContentKey.BOOKING, booking },
                { ContentKey.SELECTED_GUEST, sender?.DeepCopy() },
                { ContentKey.FROM_BILLS, true }
            };
            await CoreMethods.PushViewModel<ExtraAddViewModel>(@params);
            tcs.SetResult(true);
        }

        private  async void PayNow(GuestBill sender, TaskCompletionSource<bool> tcs)
        {

            //show confirm PiN here 
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
                        selectedGuest = sender;
                        PaymentAmount = sender.GuestDetails.Balance;
                        PaymentMethod = ListPaymentMethod.FirstOrDefault();
                        PaymentVisible = true;

                    }
                    else
                    {
                        await CoreMethods.DisplayAlert("", "You just inputed invalid PIN", "OK");
                    }
                }
            }
            else
            {
                selectedGuest = sender;
                PaymentAmount = sender.GuestDetails.Balance;
                PaymentMethod = ListPaymentMethod.FirstOrDefault();
                PaymentVisible = true;

            }
            tcs.SetResult(true);
        }

        private void OnPaymentMethodChanged()
        {
            CheckPaymentCondition();
        }

        private void PaymentAmountChanged()
        {
            CheckPaymentCondition();
        }

        private void CheckPaymentCondition()
        {
            if (PaymentMethod != null && PaymentMethod.Id.Equals(99))
            {
                CanConfirmPayment = false;
                CanPayAndCheckOut = false;
                SendPaymentLinkVisible = Math.Round(PaymentAmount, 2) >= selectedGuest.GuestDetails.Balance;
            }
            else
            {
                CanConfirmPayment = Math.Round(PaymentAmount, 2) > 0;
                CanPayAndCheckOut = Math.Round(PaymentAmount, 2) >= Math.Round(listGuest.Sum(g => g.GuestDetails.Balance), 2);
                SendPaymentLinkVisible = false;
            }
        }

        private void ConfirmPayment(GuestBill sender, TaskCompletionSource<bool> tcs)
        {
            PostPayment(sender);
            tcs.SetResult(true);
        }

        private void PayAndCheckOut(GuestBill sender, TaskCompletionSource<bool> tcs)
        {
            PostPayment(sender, true);
            tcs.SetResult(true);
        }

        private void SendPaymentLink(GuestBill sender, TaskCompletionSource<bool> tcs)
        {
            if (!ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            UserDialogs.Instance.Loading("Sending Payment Link...").Show();

            Task.Run(async () =>
            {
                return await openBookingsService.SendPaymentLink(booking.Id, sender.GuestDetails.Id, Math.Round(PaymentAmount, 2));
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

        private void ClosePayNow(GuestBill sender, TaskCompletionSource<bool> tcs)
        {
            PaymentVisible = false;
            tcs.SetResult(true);
        }

        private void PostPayment(GuestBill sender, bool checkOutQueued = false)
        {
            if (PaymentMethod == null)
            {
                CoreMethods.DisplayAlert("", "Payment method is not selected", TranslateExtension.GetValue("ok"));
                return;
            }

            if (PaymentExecuting || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            PaymentExecuting = true;
            Task.Run(async () =>
            {
                return await openBookingsService.PostPayment(booking.Id, sender.GuestDetails.Id, PaymentMethod.Id, Math.Round(PaymentAmount, 2));
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                PaymentExecuting = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            if (checkOutQueued)
                            {
                                CheckOut();
                            }
                            else
                            {
                                CoreMethods.DisplayAlert("", "Payment registered", TranslateExtension.GetValue("ok"));

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
                            await CoreMethods.DisplayAlert("", "Payment registered", TranslateExtension.GetValue("ok"));

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