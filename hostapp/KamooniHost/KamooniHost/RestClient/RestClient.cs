using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Converters;

namespace KamooniHost.RestClient
{
    public class RestClient : IRestClient
    {
        public static readonly JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Converters = new List<JsonConverter>
            {
                new IgnoreDataTypeConverter(),
                new IgnoreFalseStringConverter()
            }
        };

        private HttpClient CreateHttpClientWithChain()
        {
            // Create the client object with the topmost handler in the chain.
            return new HttpClient(new AuthenticationMessageHandler { InnerHandler = new HttpClientHandler() });
        }

        public async Task<HttpResponseMessage> GetAsync(string url, CancellationToken? token = null)
        {
            if (token != null)
                return await new HttpClient().GetAsync(url, token.Value);
            else
                return await new HttpClient().GetAsync(url);
        }

        public async Task<T> GetAsync<T>(string url, CancellationToken? token = null)
        {
            HttpResponseMessage responseMessage;

            if (token != null)
                responseMessage = await new HttpClient().GetAsync(url, token.Value);
            else
                responseMessage = await new HttpClient().GetAsync(url);

            var responseContent = await responseMessage.Content.ReadAsStringAsync();

            if (Debugger.IsAttached)
            {
                Debug.WriteLine(responseContent);
            }

            return JsonConvert.DeserializeObject<T>(responseContent, DefaultSerializerSettings);
        }

        public async Task<byte[]> GetByteArrayAsync(string url)
        {
            return await new HttpClient().GetByteArrayAsync(url);
        }

        public async Task<Stream> GetStreamAsync(string url)
        {
            return await new HttpClient().GetStreamAsync(url);
        }

        public async Task<string> GetStringAsync(string url)
        {
            return await new HttpClient().GetStringAsync(url);
        }

        public async Task<T> GetStringAsync<T>(string url)
        {
            var responseContent = await new HttpClient().GetStringAsync(url);

            if (Debugger.IsAttached)
            {
                Debug.WriteLine(responseContent);
            }

            return JsonConvert.DeserializeObject<T>(responseContent, DefaultSerializerSettings);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string url, T t, CancellationToken? token = null)
        {
            var content = JsonConvert.SerializeObject(t);

            if (Debugger.IsAttached)
            {
                Debug.WriteLine(content);
            }

            HttpContent httpContent = new StringContent(content);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            if (token != null)
                return await new HttpClient().PostAsync(url, httpContent, token.Value);
            else
                return await new HttpClient().PostAsync(url, httpContent);
        }

        public async Task<T> PostAsync<T, TP>(string url, TP t, CancellationToken? token = null)
        {
            var content = JsonConvert.SerializeObject(t);

            if (Debugger.IsAttached)
            {
                Debug.WriteLine(content);
            }

            HttpContent httpContent = new StringContent(content);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage responseMessage;

            if (token != null)
                responseMessage = await new HttpClient().PostAsync(url, httpContent, token.Value);
            else
                responseMessage = await new HttpClient().PostAsync(url, httpContent);

            var responseContent = await responseMessage.Content.ReadAsStringAsync();

            if (Debugger.IsAttached)
            {
                Debug.WriteLine(responseContent);
            }

            return JsonConvert.DeserializeObject<T>(responseContent, DefaultSerializerSettings);
        }

        public async Task<HttpResponseMessage> PostFucntionAsync<T>(string url, T t, CancellationToken? token = null)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

            var content = JsonConvert.SerializeObject(t);

            if (Debugger.IsAttached)
            {
                Debug.WriteLine(content);
            }

            HttpContent httpContent = new StringContent(content);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            if (token != null)
                return await httpClient.PostAsync(url, httpContent, token.Value);
            else
                return await httpClient.PostAsync(url, httpContent);
        }

        public async Task<T> PostFucntionAsync<T, TP>(string url, TP t, CancellationToken? token = null)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

            var content = JsonConvert.SerializeObject(t);

            if (Debugger.IsAttached)
            {
                Debug.WriteLine(content);
            }

            HttpContent httpContent = new StringContent(content);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage responseMessage;

            if (token != null)
                responseMessage = await httpClient.PostAsync(url, httpContent, token.Value);
            else
                responseMessage = await httpClient.PostAsync(url, httpContent);

            var responseContent = await responseMessage.Content.ReadAsStringAsync();

            if (Debugger.IsAttached)
            {
                Debug.WriteLine(responseContent);
            }

            return JsonConvert.DeserializeObject<T>(responseContent, DefaultSerializerSettings);
        }

        public async Task<bool> PutAsync<T>(string url, T t)
        {
            var content = JsonConvert.SerializeObject(t);

            if (Debugger.IsAttached)
            {
                Debug.WriteLine(content);
            }

            HttpContent httpContent = new StringContent(content);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return (await new HttpClient().PutAsync(url, httpContent)).IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(string url)
        {
            return (await new HttpClient().DeleteAsync(url)).IsSuccessStatusCode;
        }
    }
}