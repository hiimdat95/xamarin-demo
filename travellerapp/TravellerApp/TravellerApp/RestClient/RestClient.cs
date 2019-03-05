using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Converters;

namespace TravellerApp
{
    public class RestClient : IRestClient
    {
        public static readonly JsonSerializerSettings DefaultSerializeSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static readonly JsonSerializerSettings DefaultDeserializeSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Converters = new List<JsonConverter> { new IgnoreDataTypeConverter(), new IgnoreFalseStringConverter() }
        };

        public async Task<HttpResponseMessage> GetAsync(string url, CancellationToken? token = null)
        {
            Debug.WriteLine("GET: " + url);

            if (token != null)
                return await new HttpClient() { Timeout = TimeSpan.FromSeconds(AppSettings.RequestTimeOut) }.GetAsync(url, token.Value);
            else
                return await new HttpClient() { Timeout = TimeSpan.FromSeconds(AppSettings.RequestTimeOut) }.GetAsync(url);
        }

        public async Task<T> GetAsync<T>(string url, CancellationToken? token = null, JsonSerializerSettings deserializeSettings = null)
        {
            Debug.WriteLine("GET: " + url);

            HttpResponseMessage responseMessage;

            if (token != null)
                responseMessage = await new HttpClient() { Timeout = TimeSpan.FromSeconds(AppSettings.RequestTimeOut) }.GetAsync(url, token.Value);
            else
                responseMessage = await new HttpClient() { Timeout = TimeSpan.FromSeconds(AppSettings.RequestTimeOut) }.GetAsync(url);

            var responseContent = await responseMessage.Content.ReadAsStringAsync();
            Debug.WriteLine("RESPONSE: " + responseContent);

            if (responseMessage.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(responseContent, deserializeSettings ?? DefaultDeserializeSettings);
            }
            else
            {
                return default;
            }
        }

        public async Task<byte[]> GetByteArrayAsync(string url)
        {
            Debug.WriteLine("GET: " + url);

            return await new HttpClient() { Timeout = TimeSpan.FromSeconds(AppSettings.RequestTimeOut) }.GetByteArrayAsync(url);
        }

        public async Task<Stream> GetStreamAsync(string url)
        {
            Debug.WriteLine("GET: " + url);

            return await new HttpClient() { Timeout = TimeSpan.FromSeconds(AppSettings.RequestTimeOut) }.GetStreamAsync(url);
        }

        public async Task<string> GetStringAsync(string url)
        {
            Debug.WriteLine("GET: " + url);

            return await new HttpClient() { Timeout = TimeSpan.FromSeconds(AppSettings.RequestTimeOut) }.GetStringAsync(url);
        }

        public async Task<T> GetStringAsync<T>(string url, JsonSerializerSettings deserializeSettings = null)
        {
            Debug.WriteLine("GET: " + url);

            var responseContent = await new HttpClient() { Timeout = TimeSpan.FromSeconds(AppSettings.RequestTimeOut) }.GetStringAsync(url);
            Debug.WriteLine("RESPONSE: " + responseContent);

            return JsonConvert.DeserializeObject<T>(responseContent, deserializeSettings ?? DefaultDeserializeSettings);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string url, T t, CancellationToken? token = null, JsonSerializerSettings serializerSettings = null)
        {
            Debug.WriteLine("POST: " + url);

            var content = JsonConvert.SerializeObject(t, serializerSettings ?? DefaultSerializeSettings);

            Debug.WriteLine("CONTENT: " + content);

            HttpContent httpContent = new StringContent(content);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            if (token != null)
                return await new HttpClient() { Timeout = TimeSpan.FromSeconds(AppSettings.RequestTimeOut) }.PostAsync(url, httpContent, token.Value);
            else
                return await new HttpClient() { Timeout = TimeSpan.FromSeconds(AppSettings.RequestTimeOut) }.PostAsync(url, httpContent);
        }

        public async Task<T> PostAsync<T, TP>(string url, TP t, CancellationToken? token = null, JsonSerializerSettings serializerSettings = null, JsonSerializerSettings deserializeSettings = null)
        {
            Debug.WriteLine("POST: " + url);

            var content = JsonConvert.SerializeObject(t, serializerSettings ?? DefaultSerializeSettings);

            Debug.WriteLine("CONTENT: " + content);

            HttpContent httpContent = new StringContent(content);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage responseMessage;

            if (token != null)
                responseMessage = await new HttpClient() { Timeout = TimeSpan.FromSeconds(AppSettings.RequestTimeOut) }.PostAsync(url, httpContent, token.Value);
            else
                responseMessage = await new HttpClient() { Timeout = TimeSpan.FromSeconds(AppSettings.RequestTimeOut) }.PostAsync(url, httpContent);

            var responseContent = await responseMessage.Content.ReadAsStringAsync();
            Debug.WriteLine("RESPONSE: " + responseContent);

            if (responseMessage.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(responseContent, deserializeSettings ?? DefaultDeserializeSettings);
            }
            else
            {
                return default;
            }
        }

        public async Task<bool> PutAsync<T>(string url, T t, JsonSerializerSettings serializerSettings = null)
        {
            Debug.WriteLine("PUT: " + url);

            var content = JsonConvert.SerializeObject(t, serializerSettings ?? DefaultSerializeSettings);

            Debug.WriteLine("CONTENT: " + content);

            HttpContent httpContent = new StringContent(content);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return (await new HttpClient() { Timeout = TimeSpan.FromSeconds(AppSettings.RequestTimeOut) }.PutAsync(url, httpContent)).IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(string url)
        {
            Debug.WriteLine("DELETE: " + url);

            return (await new HttpClient() { Timeout = TimeSpan.FromSeconds(AppSettings.RequestTimeOut) }.DeleteAsync(url)).IsSuccessStatusCode;
        }
    }
}