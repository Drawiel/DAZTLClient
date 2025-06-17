using DAZTLClient.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DAZTLClient.Services
{
    public class WebSocketClientChat
    {
        
        private readonly ClientWebSocket _client = new();
        private readonly Uri _uri = new("ws://localhost:8000/ws/chat/");
        private ChatWindow chatWindow;

        public WebSocketClientChat(ChatWindow chatwindow)
        {
            this.chatWindow = chatWindow;
        }

        public async Task StartAsync()
            {
                try
                {
                    await _client.ConnectAsync(_uri, CancellationToken.None);
                    Console.WriteLine("Conectado al WebSocket");

                    byte[] buffer = new byte[1024];

                    while (_client.State == WebSocketState.Open)
                    {
                        var result = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            chatWindow.PrintMessage(message);
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al conectar WebSocket: " + ex.Message);
                }
            }

    }
}
