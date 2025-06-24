using DAZTLClient.Models;
using DAZTLClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DAZTLClient.Windows
{
    /// <summary>
    /// Lógica de interacción para ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        public ChatWindow(string message)
        {
            this.Title = "Chat en vivo";
            InitializeComponent();
            string modifiedMessage = message.Replace(
                "Únete al chat para ver que piensan de ella",
                "Conversa con los demás oyentes para ver que opinan"
            );
            AddMessage("Daztl", modifiedMessage);
            _ = StartWebSocketListener();
        }
        private async Task StartWebSocketListener()
        {
            var client = new WebSocketClientChat(this);
            await client.StartAsync();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string userInput = InputBox.Text.Trim();
            if (!string.IsNullOrEmpty(userInput))
            {
                InputBox.Clear();
                _=SendMessageAsync(userInput);
            }
        }

        private async Task SendMessageAsync(string userInput)
        {
            await new ContentService().SendChatMessageAsync(1, userInput);

        }

        private void AddMessage(string sender, string message)
        {
            var textBlock = new TextBlock
            {
                Text = $"{sender}: {message}",
                Foreground = Brushes.White,
                Margin = new Thickness(0, 5, 0, 5),
                TextWrapping = TextWrapping.Wrap
            };

            MessagesPanel.Children.Add(textBlock);
        }

        public void PrintMessage(string message)
        {
            var messageChat = JsonSerializer.Deserialize<MessageChatUnmarshalling>(message);

            if (messageChat?.type == "chat_message")
            {
                AddMessage(messageChat.username, messageChat.message);
            }
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
