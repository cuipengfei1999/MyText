using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace MyMainWinods.ViewMoldes
{
    public partial class MainWindowViewModel : ObservableObject
    {

        static HttpClient _client = new HttpClient();
        static Uri _uri;
        public MainWindowViewModel()
        {
            SendContent = "<WPF发送数据>";
            ReceiveMessage = $"接收的消息:{Environment.NewLine}";
            Users = new ObservableCollection<User>();
            _client.BaseAddress = new Uri("https://localhost:9999/");
            //_uri = new Uri($@"wss://localhost:9999/ws");
            _uri = new Uri($@"ws://localhost:8888");
        }

        [ObservableProperty]
        private string sendContent;
        [ObservableProperty]
        private string receiveMessage;
        [ObservableProperty]
        private ObservableCollection<User> users;

        [RelayCommand]
        async void JsonClick()
        {
            // 实例化一个文件选择对象
            var dialog = new Microsoft.Win32.OpenFileDialog()
            {
                DefaultExt = ".json",// 设置默认类型
                Filter = @"json文件(*.json)|*.json",// 设置可选格式
            };

            var result = dialog.ShowDialog();// 打开选择框选择
            if (result == true)
            {
                var file = dialog.FileName; // 获取选择的文件名

                using var reader = File.OpenText(file);
                var contents = await reader.ReadToEndAsync();
                Users = JsonConvert.DeserializeObject<ObservableCollection<User>>(contents);
            }
        }

        [RelayCommand]
        async void GetClick()
        {
            var response = await _client.GetAsync("api/User/GetUsers");
            if (response.IsSuccessStatusCode)
            {
                Users = await response.Content.ReadFromJsonAsync<ObservableCollection<User>>();
                MessageBox.Show("Get OK");
            }
        }

        [RelayCommand]
        async void PostClick()
        {
            if (Users.Count > 0)
            {
                var response = await _client.PostAsync("api/User/AddUsers",
                    new StringContent(JsonConvert.SerializeObject(Users),
                    Encoding.UTF8,
                    "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Post OK");
                }
            }
            else
            {
                MessageBox.Show("Fail:List Is Null!");
            }
        }

        [RelayCommand]
        async void SendClick()
        {
            using var clientWebSocket = new ClientWebSocket();
             var cancellation = new CancellationToken();

            try
            {
                await clientWebSocket.ConnectAsync(_uri, cancellation);
                await Echo(clientWebSocket, SendContent);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"{DateTime.Now:yyyy年dd月MM日 HH:mm:ss}");
                sb.AppendLine($"消息:{SendContent}");
                ReceiveMessage += sb;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message); throw;
            }
        }

        /// <summary>
        /// Send and receive messages
        /// </summary>
        /// <param name="webSocket"></param>
        /// <returns></returns>

        private static async Task Echo(ClientWebSocket webSocket, string message)
        {

            await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Binary, true, CancellationToken.None);

            var incomingData = new byte[1024];
            //var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(incomingData), CancellationToken.None);
            //await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "1", CancellationToken.None);

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "1", CancellationToken.None);

        }



    }
}
