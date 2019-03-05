using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using KamooniHost.Models.Response;
using KamooniHost.Models.Result;
using KamooniHost.RestClient;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace KamooniHost.Services
{
    public class GuestBookService : IGuestBookService
    {
        private readonly IRestClient restClient;

        public GuestBookService(IRestClient restClient)
        {
            this.restClient = restClient;
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

        public async Task<GuestScanResult> ManualCheckIn(GuestCheckIn guest)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", Settings.CurrentHost.Token),
                    new JProperty("guest", new JObject()
                    {
                        new JProperty("name", guest.Name),
                        new JProperty("mobile", guest.Mobile),
                        new JProperty("email", guest.Email),
                        new JProperty("passport", guest.Passport),
                        new JProperty("passport_image", guest.PassportImage),
                        new JProperty("country", guest.Country)
                    })
                })
            };

            GuestScanResponse response = await restClient.PostAsync<GuestScanResponse, JObject>(Constants.ApiURI.URL_MAIN + Constants.ApiURI.MANUAL_CHECK_IN, @params);

            return response?.Result;
        }

        public async Task<GuestBookResult> GetGuestBooks()
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("host", new JObject()
                    {
                        new JProperty("token", Settings.CurrentHost.Token)
                    })
                })
            };

            GuestBookResponse response = await restClient.PostAsync<GuestBookResponse, JObject>(Constants.ApiURI.URL_MAIN + Constants.ApiURI.GET_GUEST_BOOK, @params);

            return response?.Result;
        }

        public async Task<GuestProfileResult> GetGuestProfile(int id)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("host_token", Settings.CurrentHost.Token),
                    new JProperty("traveller_partner_id", id)
                })
            };

            GuestProfileResponse response = await restClient.PostAsync<GuestProfileResponse, JObject>(Constants.ApiURI.URL_MAIN + Constants.ApiURI.GET_GUEST_PROFILE, @params);

            return response?.Result;
        }

        public async Task<PostResult> GuestCheckOut(string guestToken)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("host", new JObject()
                    {
                        new JProperty("usr_token", Settings.CurrentHost.Token),
                        new JProperty("visit_token", guestToken)
                    })
                })
            };

            PostResponse response = await restClient.PostAsync<PostResponse, JObject>(Constants.ApiURI.URL_MAIN + Constants.ApiURI.GUEST_BOOK_CHECK_OUT, @params);

            return response?.Result;
        }

        public async Task<PostResult> DownVoteGuest(string guestToken, string note)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("host_token", Settings.CurrentHost.Token),
                    new JProperty("guest_token", guestToken),
                    new JProperty("note", note)
                })
            };

            PostResponse response = await restClient.PostAsync<PostResponse, JObject>(Constants.ApiURI.URL_MAIN + Constants.ApiURI.GUEST_DOWN_VOTE, @params);

            return response?.Result;
        }

        public async Task<PostResult> UpVoteGuest(string guestToken)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("host_token", Settings.CurrentHost.Token),
                    new JProperty("guest_token", guestToken)
                })
            };

            PostResponse response = await restClient.PostAsync<PostResponse, JObject>(Constants.ApiURI.URL_MAIN + Constants.ApiURI.GUEST_UP_VOTE, @params);

            return response?.Result;
        }
    }
}