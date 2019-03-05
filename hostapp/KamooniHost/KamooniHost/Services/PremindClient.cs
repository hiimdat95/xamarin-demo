using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models.Profile;
using KamooniHost.Views.Popup;
using Newtonsoft.Json;
using Plugin.LocalNotifications;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Premind
{
    public static class Constants
    {
        public static string ON_RECEIVED = "ON_RECEIVED";
    }

    public static class PremindClient
    {
        private static ClientWebSocket client = new ClientWebSocket();
        private static CancellationTokenSource cts;

        public static event EventHandler<string> OnReceived;

        public static async Task<bool> StartClient()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Settings.PremindUri))
                {
                    if (await StartClient(new Uri(Settings.PremindUri)))
                    {
                        OnReceived += (s, e) =>
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                var result = JsonConvert.DeserializeObject<Profile>(e);
                                CrossLocalNotifications.Current.Show(result.Nickname, result.Visit?.ToString());
                                DependencyService.Get<IPremindService>()?.InitContent(new PremindNotification(result.Image));
                                DependencyService.Get<IPremindService>()?.ShowContent();
                            });
                        };

                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connect ES Error: {0}", ex.Message);
                return false;
            }
        }

        public static async Task<bool> StartClient(Uri uri)
        {
            try
            {
                if (client.State == WebSocketState.Open || client.State == WebSocketState.Connecting)
                    return false;

                if (cts != null)
                    cts.Cancel();

                cts = new CancellationTokenSource();
                var token = cts.Token;

                await client.ConnectAsync(uri, token);

                await Task.Factory.StartNew(async () =>
                {
                    while (true)
                    {
                        await ReadMessage(token);
                    }
                }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connect ES Error: {0}", ex.Message);
                return false;
            }
        }

        public static async Task ReadMessage(CancellationToken token)
        {
            WebSocketReceiveResult result;
            var readBuffer = new ArraySegment<byte>(new byte[4096]);
            do
            {
                result = await client.ReceiveAsync(readBuffer, token);

                if (result.MessageType != WebSocketMessageType.Text)
                    break;

                var messageBytes = readBuffer.Skip(readBuffer.Offset).Take(result.Count).ToArray();
                string receivedMessage = Encoding.UTF8.GetString(messageBytes);

                Console.WriteLine("Received: {0}", receivedMessage);

                OnReceived?.Invoke(typeof(PremindClient), receivedMessage);
                MessagingCenter.Send(typeof(PremindClient), Constants.ON_RECEIVED, receivedMessage);
            }
            while (!result.EndOfMessage);
        }
    }
}