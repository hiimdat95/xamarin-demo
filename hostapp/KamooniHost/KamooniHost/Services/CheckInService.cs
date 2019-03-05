using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using KamooniHost.Models.Response;
using KamooniHost.Models.Result;
using KamooniHost.RestClient;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace KamooniHost.Services
{
    public class CheckInService : ICheckInService
    {
        private readonly IRestClient restClient;

        public CheckInService(IRestClient restClient)
        {
            this.restClient = restClient;
        }

        public async Task<ToCheckInResult> GetCheckInBookings(DateTime startDate, DateTime endDate)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("token", Settings.CurrentHost.Token)
                    }),
                    new JProperty("filters", new JObject()
                    {
                        new JProperty("start_date", startDate),
                        new JProperty("end_date", endDate)
                    })
                })
            };

            ToCheckInResponse response = await restClient.PostAsync<ToCheckInResponse, JObject>(Settings.CurrentHost.Url + Constants.ApiURI.URI_FETCH_BOOKING, @params);

            return response?.Result;
        }

        public async Task<CheckInFormResult> GetGuestDetails(string bookingId)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("token", Settings.CurrentHost.Token)
                    }),
                    new JProperty("booking", new JObject()
                    {
                        new JProperty("booking_id", bookingId)
                    })
                })
            };

            CheckInFormResponse response = await restClient.PostAsync<CheckInFormResponse, JObject>(Settings.CurrentHost.Url + Constants.ApiURI.URI_GET_CHECK_IN_FORM, @params);

            return response?.Result;
        }

        public async Task<TravellerFromTokenResult> GuestDetailsFromQR(string travellerToken)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("token", Settings.CurrentHost.Token)
                    }),
                    new JProperty("token", travellerToken)
                })
            };

            TravellerFromTokenResponse response = await restClient.PostAsync<TravellerFromTokenResponse, JObject>(Constants.ApiURI.URL_MAIN + Constants.ApiURI.GET_TRAVELLER_FROM_TOKEN, @params);

            return response?.Result;
        }

        public async Task<GuestScanResult> ScanGuest(string guestToken)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("host_token", Settings.CurrentHost.Token),
                    new JProperty("guest_token", guestToken)
                })
            };

            GuestScanResponse response = await restClient.PostAsync<GuestScanResponse, JObject>(Constants.ApiURI.URL_MAIN + Constants.ApiURI.GUEST_BOOK_SCAN_GUEST, @params);

            return response?.Result;
        }

        public async Task<PostResult> PostGuestDetails(CheckInFormItem checkInFormItem)
        {
            JArray guest_list = new JArray
            {
                new JObject()
                {
                    new JProperty("id", checkInFormItem.Id),
                    new JProperty("name", checkInFormItem.Name),
                    new JProperty("email", checkInFormItem.Email),
                    new JProperty("mobile", checkInFormItem.Mobile),
                    new JProperty("passport_id", checkInFormItem.PassportId),
                    new JProperty("passport_image", checkInFormItem.PassportImage),
                    new JProperty("country_iso", checkInFormItem.CountryISO),
                    new JProperty("room", checkInFormItem.Room),
                    new JProperty("token", checkInFormItem.GuestToken)
                }
            };

            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("token", Settings.CurrentHost.Token)
                    }),
                    new JProperty("guest_list", guest_list)
                })
            };

            PostResponse response = await restClient.PostAsync<PostResponse, JObject>(Settings.CurrentHost.Url + Constants.ApiURI.URI_UPDATE_CHECK_IN_FORM, @params);

            return response?.Result;
        }

        public async Task<PostResult> ConfirmCheckIn(string bookingId)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("token", Settings.CurrentHost.Token)
                    }),
                    new JProperty("booking", new JObject()
                    {
                        new JProperty("booking_id", bookingId)
                    })
                })
            };

            PostResponse response = await restClient.PostAsync<PostResponse, JObject>(Settings.CurrentHost.Url + Constants.ApiURI.URI_CONFIRM_CHECK_IN, @params);

            return response?.Result;
        }
    }
}