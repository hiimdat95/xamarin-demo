using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace KamooniHost.RestClient
{
    public interface IRestClient
    {
        Task<HttpResponseMessage> GetAsync(string url, CancellationToken? token = null);

        Task<T> GetAsync<T>(string url, CancellationToken? token = null);

        Task<byte[]> GetByteArrayAsync(string url);

        Task<Stream> GetStreamAsync(string url);

        Task<string> GetStringAsync(string url);

        Task<T> GetStringAsync<T>(string url);

        Task<HttpResponseMessage> PostAsync<T>(string url, T t, CancellationToken? token = null);

        Task<T> PostAsync<T, TP>(string url, TP t, CancellationToken? token = null);

        Task<HttpResponseMessage> PostFucntionAsync<T>(string url, T t, CancellationToken? token = null);

        Task<T> PostFucntionAsync<T, TP>(string url, TP t, CancellationToken? token = null);

        Task<bool> PutAsync<T>(string url, T t);

        Task<bool> DeleteAsync(string url);
    }
}