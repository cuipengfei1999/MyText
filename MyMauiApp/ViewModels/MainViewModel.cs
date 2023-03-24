using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMauiApp.Models;
using Microsoft.Maui.Controls;

namespace MyMauiApp.ViewModels
{
    public partial class MainViewModel: ObservableObject
    {
        static HttpClient _client = new HttpClient();
        public MainViewModel() 
        {
            SendContent = "<MAUI发送数据>";
            ReceiveMessage = $"接收数据:{Environment.NewLine}";
            Users = new ObservableCollection<User>();
            _client.BaseAddress = new Uri("https://localhost:9999");
        }

        [ObservableProperty]
        private string sendContent;
        [ObservableProperty]
        private string receiveMessage;
        [ObservableProperty]
        private ObservableCollection<User> users;


        [RelayCommand]
        async void SendClick()
        {
            var clientWebSocket = new ClientWebSocket();
            var cancellation = new CancellationToken();
            try
            {
                var uri = new Uri($@"ws://localhost:8888");
                await clientWebSocket.ConnectAsync(uri, cancellation);
                await Echo(clientWebSocket, SendContent);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"{DateTime.Now:yyyy年dd月MM日 HH:mm:ss}");
                sb.AppendLine($"消息:{SendContent}");
                ReceiveMessage = sb.ToString();
            }
            catch (Exception e)
            {
                await new Page().DisplayAlert("error", e.Message, "OK");
                throw;
            }

        }

        [RelayCommand]
        async void GetJsonClick()
        {
            var response = await _client.GetAsync("api/User/GetUsers");
            if (response.IsSuccessStatusCode)
            {
                Users = await response.Content.ReadFromJsonAsync<ObservableCollection<User>>();
            }
        }


        /// <summary>
        /// Send and receive messages
        /// </summary>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        private static async Task Echo(WebSocket webSocket,string message)
        {
            await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)),WebSocketMessageType.Binary,true,CancellationToken.None);

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,"1",CancellationToken.None);
        }

    }
}
