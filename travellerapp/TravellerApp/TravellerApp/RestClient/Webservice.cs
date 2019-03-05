using Acr.UserDialogs;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TravellerApp.Notifications;
using Xamarin.Forms;

namespace TravellerApp
{
    public class WebService
    {
        private static readonly Lazy<WebService> lazy = new Lazy<WebService>(() => new WebService());

        public static WebService Instance => lazy.Value;

        private WebService()
        {
        }

        public async Task GetAsync<T>(string url, string loadingMessage = null, Action<T> onSuccess = null, Action onError = null)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(loadingMessage))
                    UserDialogs.Instance.Loading(loadingMessage).Show();

                var result = await DependencyService.Get<IRestClient>().GetAsync<T>(url);

                onSuccess?.Invoke(result);
            }
            catch (TaskCanceledException e)
            {
                Internal.BadConnectionError();
                Debug.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Internal.ServerError();
                Debug.WriteLine(e.Message);
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(loadingMessage))
                    UserDialogs.Instance.Loading().Hide();
            }
        }

        public async Task PostAsync<T>(string url, string loadingMessage = null, object content = null, Action<T> onSuccess = null, Action onError = null)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(loadingMessage))
                    UserDialogs.Instance.Loading(loadingMessage).Show();

                var result = await DependencyService.Get<IRestClient>().PostAsync<T, object>(url, content);

                onSuccess?.Invoke(result);
            }
            catch (TaskCanceledException e)
            {
                Internal.BadConnectionError();
                Debug.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Internal.ServerError();
                Debug.WriteLine(e.Message);
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(loadingMessage))
                    UserDialogs.Instance.Loading().Hide();
            }
        }

        public void GetAsyncWithTask<T>(string url, string loadingMessage = null, Action<T> onSuccess = null, Action onError = null)
        {
            if (!string.IsNullOrWhiteSpace(loadingMessage))
                UserDialogs.Instance.Loading(loadingMessage).Show();

            Task.Run(async () =>
            {
                return await DependencyService.Get<IRestClient>().GetAsync<T>(url);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                if (!string.IsNullOrWhiteSpace(loadingMessage))
                    UserDialogs.Instance.Loading().Hide();

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    onSuccess?.Invoke(task.Result);
                }
                else if (task.IsCanceled)
                {
                    Internal.BadConnectionError();
                    Debug.WriteLine(task);
                }
                else if (task.IsFaulted)
                {
                    onError?.Invoke();
                    Internal.ServerError();
                    Debug.WriteLine(task.Exception.GetBaseException()?.Message);
                }
            }));
        }

        public void PostAsyncWithTask<T>(string url, object content = null, string loadingMessage = null, Action<T> onSuccess = null, Action onError = null)
        {
            if (!string.IsNullOrWhiteSpace(loadingMessage))
                UserDialogs.Instance.Loading(loadingMessage).Show();

            Task.Run(async () =>
            {
                return await DependencyService.Get<IRestClient>().PostAsync<T, object>(url, content);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                if (!string.IsNullOrWhiteSpace(loadingMessage))
                    UserDialogs.Instance.Loading().Hide();

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    onSuccess?.Invoke(task.Result);
                }
                else if (task.IsCanceled)
                {
                    Internal.BadConnectionError();
                    Debug.WriteLine(task);
                }
                else if (task.IsFaulted)
                {
                    onError?.Invoke();
                    Internal.ServerError();
                    Debug.WriteLine(task.Exception.GetBaseException()?.Message);
                }
            }));
        }
    }
}