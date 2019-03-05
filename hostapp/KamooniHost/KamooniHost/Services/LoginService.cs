using KamooniHost.Constants;
using KamooniHost.IServices;
using KamooniHost.Models.Profile.Auth;
using KamooniHost.Models.Response;
using KamooniHost.Models.Result;
using KamooniHost.RestClient;
using KamooniHost.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace KamooniHost.Services
{
    public class LoginService : ILoginService
    {
        private readonly IRestClient restClient;

        public LoginService(IRestClient restClient)
        {
            this.restClient = restClient;
        }

        public async Task<UserExistCheckResult> CheckIfExist(string email)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("email", email.ToLower().Trim())
                })
            };

            UserExistCheckResponse response = await restClient.PostAsync<UserExistCheckResponse, JObject>(ApiURI.URL_MAIN + ApiURI.URI_CHECK_USER, @params);

            return response?.Result;
        }

        public async Task<HostResult> Login(string email, string password)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("db", "kamooni"),
                        new JProperty("login", email.ToLower().Trim()),
                        new JProperty("password", StringFormatUtil.ToBase64(password))
                    })
                })
            };

            HostResponse response = await restClient.PostAsync<HostResponse, JObject>(ApiURI.URL_MAIN + ApiURI.URI_SIGN_IN, @params);

            return response?.Result;
        }

        public async Task<string> PremindLogin(string email, string password)
        {
            JObject @params = new JObject()
            {
               new JProperty("username", email.ToLower().Trim()),
               new JProperty("password", password)
            };

            var responseMessage = await restClient.PostAsync(ApiURI.URI_WEBSOCKET + ApiURI.URI_WEBSOCKET_LOGIN, @params);

            if (responseMessage.IsSuccessStatusCode)
            {
                Auth response = JsonConvert.DeserializeObject<Auth>(await responseMessage.Content.ReadAsStringAsync(), RestClient.RestClient.DefaultSerializerSettings);

                if (response != null && !string.IsNullOrWhiteSpace(response.WsScheme)
                    && !string.IsNullOrWhiteSpace(response.GroupName)
                    && !string.IsNullOrWhiteSpace(response.Host)
                    && !string.IsNullOrWhiteSpace(response.Scheme)
                    && !string.IsNullOrWhiteSpace(response.Token))
                {
                    return string.Format("{0}://pre-mind.com/?room={1}&host={2}&protocol={3}&token={4}", response.WsScheme, response.GroupName, response.Host, response.Scheme, response.Token);
                }
            }

            return string.Empty;
        }

        public async Task<SuccessCheckResult> ForgotPassword(string email)
        {
            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("email", email)
                })
            };

            SuccessCheckResponse response = await restClient.PostAsync<SuccessCheckResponse, JObject>(ApiURI.URL_MAIN + ApiURI.URI_FORGOT_PASSWORD, @params);

            return response?.Result;
        }
    }
}