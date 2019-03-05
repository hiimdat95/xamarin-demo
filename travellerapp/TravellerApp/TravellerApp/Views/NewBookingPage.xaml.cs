using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using FFImageLoading.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Realms;
using TravellerApp.Constants;
using TravellerApp.Helpers;
using TravellerApp.Interfaces;
using TravellerApp.Models;
using TravellerApp.Models.Response;
using TravellerApp.Models.Result;
using TravellerApp.Notifications;
using Xamarin.Forms;

namespace TravellerApp.Views
{
    public partial class NewBookingPage : ContentPage
    {
        private ListInfo listInfo;
        private User user;
        private GetAvailableActivitiesResponse getAvailableActivitiesResponse;
        private double totalPay = 0;
        private List<PeriodId> listSelectedPeriod = new List<PeriodId>();
        List<Picker> listPicker = new List<Picker>();
        public NewBookingPage(ListInfo data)
        {
            InitializeComponent();

            this.listInfo = data;
            this.Title = this.listInfo.name;
            //Get User
            var realm = Realm.GetInstance();
            user = realm.Find<User>(DBLocalID.USER);

            this.LoadActivities();
        }

        private void UpdateLayout(ActivitiesResult activitiesResult)
        {
            buttonPicker.Text = activitiesResult.activity_name;
            imgActivity.Source = activitiesResult.image_url;
            lbSummary.Text = activitiesResult.summary;
            var OrderPeriodList = activitiesResult.period_ids.OrderByDescending(d => d.start_date).ToList();

            //lvPeriods.ItemsSource = OrderPeriodList;
            var groupedPeriodList = OrderPeriodList.GroupBy(u => u.title_date)
                                      .Select(grp => new { Id = grp.Key, ListPeriod = grp.ToList() })
                                      .ToList();

            lvPeriods.ItemsSource = groupedPeriodList;



        }
        private void createBookingActivity(object sender, EventArgs e)
        {
            if (listSelectedPeriod.Count > 0)
            {
                CreateBooking();
            }
            //
        }
        private void OnCancelItemClick(object sender, EventArgs e)
        {

            var selectedItem = ((CachedImage)sender).BindingContext as PeriodId;
            if (listBookedPeriod.ItemsSource is List<PeriodId> listPeriod)
            {
                if (listPeriod.Count > 0)
                {
                    listSelectedPeriod = listSelectedPeriod.Where(x => x.period_id != selectedItem.period_id).ToList();
                }

                //calculator again
                totalPay = 0;
                foreach (var item in listSelectedPeriod)
                {

                    totalPay += item.price * item.selected_person;
                }

                listBookedPeriod.ItemsSource = listSelectedPeriod;
                total_pay.Text = totalPay.ToString();

                //selectedItem.selected_person = 0;

                frCheckOut.IsVisible = (!string.IsNullOrEmpty(total_pay.Text) && !"0".Equals(total_pay.Text)) ? true : false;
            }
            foreach(var itemPicker in listPicker.ToList())
            {
                if(itemPicker.BindingContext is PeriodId period){ 
                    if(period.period_id == selectedItem.period_id) { 
                        itemPicker.SelectedIndex = -1;
                        break;
                    }
                }
            }


        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private void SelectActivityClicked(object sender, EventArgs e)
        {
            pickerActivity.Focus();
        }

        private void ChooseClicked(object sender, EventArgs e)
        {

            if (((Picker)sender).BindingContext is PeriodId selectedItem)
            {
                //((Picker)sender).SelectedIndex = -1;
             
                listBookedPeriod.ItemsSource = new List<PeriodId>();
                selectedItem.selected_person = ((Picker)sender).SelectedIndex + 1;
                var data = getAvailableActivitiesResponse.Result[pickerActivity.SelectedIndex];
                selectedItem.activity_id = data.activity_id;
                selectedItem.activity_name = data.activity_name;
                listSelectedPeriod = listSelectedPeriod.Where(i => (i.period_id != selectedItem.period_id && i.selected_person>0)).ToList();

                //bool isAdd = false;
                //for (int i = 0; i < listSelectedPeriod.Count; i++)
                //{
                //    if (listSelectedPeriod[i].period_id == selectedItem.period_id)
                //    {
                //        isAdd = true;
                //        listSelectedPeriod[i] = selectedItem;
                //        break;
                //    }
                //}

                //if (!isAdd)
                //{
                //    listSelectedPeriod.Add(selectedItem);
                //}

                //var temp = new List<PeriodId>();
                //for (int i = 0; i < listSelectedPeriod.Count; i++)
                //{
                //    if (temp.Count == 0)
                //    {
                //        listSelectedPeriod[i].price = listSelectedPeriod[i].price * listSelectedPeriod[i].selected_person;
                //        temp.Add(listSelectedPeriod[i]);
                //    }
                //    else
                //    {
                //        bool isExits = false;
                //        for (int j = 0; j < temp.Count; j++)
                //        {
                //            if (listSelectedPeriod[i].activity_id == temp[j].activity_id)
                //            {
                //                isExits = true;
                //                temp[j].price += listSelectedPeriod[i].price * listSelectedPeriod[i].selected_person;
                //                break;
                //            }
                //        }
                //        if (!isExits)
                //        {
                //            listSelectedPeriod[i].price += listSelectedPeriod[i].price * listSelectedPeriod[i].selected_person;
                //            temp.Add(listSelectedPeriod[i]);
                //        }
                //    }
                //}
                if(selectedItem.selected_person>0)
                    listSelectedPeriod.Add(selectedItem);

                totalPay = 0;
                //calculator
                foreach (var item in listSelectedPeriod)
                {
                    item.totalPrice = item.price * item.selected_person;
                    totalPay += item.price * item.selected_person;
                }

                listBookedPeriod.ItemsSource = listSelectedPeriod;

                total_pay.Text = totalPay.ToString();

                frCheckOut.IsVisible = (!string.IsNullOrEmpty(total_pay.Text) && !"0".Equals(total_pay.Text)) ? true : false;
                //frCheckOut.IsVisible = true;
                listPicker.Add((Picker)sender);
            }
        }

        void OnPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                this.UpdateLayout(this.getAvailableActivitiesResponse.Result[selectedIndex]);

            }
        }

        private async void LoadActivities()
        {
            try
            {
                UserDialogs.Instance.Loading(title: "Load Activities ...").Show();
                var client = new HttpClient()
                {
                    Timeout = TimeSpan.FromSeconds(30)
                };

                JObject jsonDataObject = new JObject
                {
                    { "auth", new JObject{
                        { "token", this.listInfo.usr_token }
                    } }
                };

                JObject jsonData = new JObject
                {
                    { "params", jsonDataObject }
                };

                var data = @jsonData.ToString();
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(this.listInfo.host_url + ApiUri.LOAD_ACTIVITIES, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    this.getAvailableActivitiesResponse = JsonConvert.DeserializeObject<GetAvailableActivitiesResponse>(responseContent, App.DefaultSerializerSettings);

                    if (this.getAvailableActivitiesResponse != null && this.getAvailableActivitiesResponse.Result != null && this.getAvailableActivitiesResponse.Result.Count > 0)
                    {
                        pickerActivity.ItemsSource = this.getAvailableActivitiesResponse.Result;
                        pickerActivity.SelectedIndex = 0;
                        pickerActivity.SelectedIndexChanged += this.OnPickerSelectedIndexChanged;
                        this.UpdateLayout(this.getAvailableActivitiesResponse.Result[0]);

                    }
                }
                else
                {
                    Internal.ServerError();
                }
            }
            catch (TaskCanceledException e)
            {
                UserDialogs.Instance.Toast(new ToastConfig("Bad Connection Error. Try Again"));
            }
            catch (Exception e)
            {
                Internal.ServerError();
            }
            finally
            {
                UserDialogs.Instance.Loading().Hide();
            }
        }

        private async void CreateBooking()
        {
            try
            {
                UserDialogs.Instance.Loading(title: "Create Booking ...").Show();
                var client = new HttpClient()
                {
                    Timeout = TimeSpan.FromSeconds(30)
                };
                JArray activityDataObject = new JArray();
                foreach (var item in listSelectedPeriod)
                {
                    activityDataObject.Add(new JObject()
                                                {
                                                    { "activity_id",Int32.Parse(item.activity_id) },
                                                    { "period_id", item.period_id },
                                                    { "persons", item.selected_person }
                                                });
                }

                JObject customerDataObject = new JObject
                {
                    { "name", user.name },
                    { "mobile", user.mobile },
                    { "email", user.email },
                    { "token", user.traveller_token}
                };

                JObject sourceDataDict = new JObject
                {
                    { "source", "Kamooni Traveller App" },
                    { "channel", "Kamooni Traveller App"},
                };
                JObject sourceData = new JObject
                {
                    { "source",sourceDataDict }
                };
                JObject token = new JObject
                {
                    {"token",this.listInfo.usr_token},
                    {"url",this.listInfo.host_url},
                };

                JObject pramsData = new JObject
                {
                    { "auth", token },
                    { "activitydata", activityDataObject },
                    { "customer", customerDataObject },
                    { "source", sourceDataDict }

                };

                JObject jsonData = new JObject
                {
                    { "params", pramsData }
                };

                var data = jsonData.ToString();
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.CREATE_ACTIVITY_BOOKING, content);

                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    NewBookingResponse newBookingResponse = JsonConvert.DeserializeObject<NewBookingResponse>(responseContent, App.DefaultSerializerSettings);
                    if (newBookingResponse.Result != null && newBookingResponse.Result.success)
                    {
                        if (!string.IsNullOrWhiteSpace(newBookingResponse.Result.peach_payment_link_url))
                        {
                            var msgCreatBooking = "Your reservation is almost confirmed! Please click pay to confirm your booking. A payment link is also email to you.";
                            await DisplayAlert("Booking Created Successfully", msgCreatBooking, "Pay Now");
                            //Open PayNow
                            Device.OpenUri(new Uri(newBookingResponse.Result.peach_payment_link_url));

                            var page = new HomePage();
                            await Navigation.PushAsync(page, true);
                        }
                        else
                        {
                            var msg = "Payment Link Not Available";
                            await DisplayAlert("Confirm", msg, "OK");
                        }
                    }
                    else
                    {
                        var msgCreatBooking = newBookingResponse.Result.message;
                        await DisplayAlert("Booking Created Unsuccessfully", msgCreatBooking, "OK");
                    }
                }
                else
                {
                    Internal.ServerError();
                }
            }
            catch (TaskCanceledException e)
            {
                UserDialogs.Instance.Toast(new ToastConfig("Bad Connection Error. Try Again"));
            }
            catch (Exception e)
            {
                Internal.ServerError();
            }
            finally
            {
                UserDialogs.Instance.Loading().Hide();
            }
        }
    }
}
