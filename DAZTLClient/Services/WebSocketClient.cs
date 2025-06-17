using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DAZTLClient.Services
{
    public class WebSocketClient
    {
        
            private readonly ClientWebSocket _client = new();
            private readonly Uri _uri = new("ws://localhost:8000/ws/notifications/");

            public async Task StartAsync()
            {
                try
                {
                    await _client.ConnectAsync(_uri, CancellationToken.None);
                    Console.WriteLine("✅ Conectado al WebSocket");

                    byte[] buffer = new byte[1024];

                    while (_client.State == WebSocketState.Open)
                    {
                        var result = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                        // Lógica para manejar el mensaje recibido
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show($"📩 Notificación: {message}");
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("❌ Error al conectar WebSocket: " + ex.Message);
                }
            }

    }
}
