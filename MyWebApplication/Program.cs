using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyWebApplication.Contexts;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;

namespace MyWebApplication
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        static ICollection<WebSocket> _webSockets = new List<WebSocket>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Console.WriteLine("MyWebAPI MyWebAPI MyWebAPI MyWebAPI MyWebAPI MyWebAPI");
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddControllersWithViews();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                // using System.Reflection;
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                options.OrderActionsBy(o => o.RelativePath);
            });

            builder.Services.AddDbContext<DataContext>(options =>
            {
                string? connectionString = builder.Configuration.GetConnectionString("WebApiDatabase");
                options.UseSqlite(connectionString);
            });

            var app = builder.Build();

            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromMinutes(5)
            };

            app.UseWebSockets(webSocketOptions);

            app.Use(async (context, next) =>
            {

                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    _webSockets.Add(webSocket);

                    string receiveMessage = await Echo(webSocket, "Hello");

                    await Console.Out.WriteLineAsync($"ReceiveMessage:{receiveMessage}{Environment.NewLine}");
                }
                else
                {
                    //context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await next(context);
                }

            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }


        private static async Task<string> Echo(WebSocket webSocket,string message)
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
            var result = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);

            while (!receiveResult.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(
                    new ArraySegment<byte>(Encoding.UTF8.GetBytes(message), 0, message.Length),
                    receiveResult.MessageType,
                    receiveResult.EndOfMessage,
                    CancellationToken.None);

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);

            return result;
        }


        private static async Task<string> Echo2(WebSocket webSocket, string? message)
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            string result = Encoding.UTF8.GetString(buffer.ToArray());

            while (!receiveResult.CloseStatus.HasValue)
            {
                foreach (var socket in _webSockets)
                {
                    await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message), 0, message.Length),
                        receiveResult.MessageType, receiveResult.EndOfMessage, CancellationToken.None);
                }
                
                var receiveResult2 = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);

            _webSockets.Remove(webSocket);

            return result;
        }

    }
}