using DAZTLClient.Windows.UserControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static DAZTLClient.Windows.HomeListeners;


namespace DAZTLClient.Windows
{
    /// <summary>
    /// Lógica de interacción para GUI_ListenersPlaylist.xaml
    /// </summary>
    public partial class GUI_ListenersPlaylist : Page {
        private List<Notification> notifications = new List<Notification>();
        private bool hasUnreadNotifications = true;
        public GUI_ListenersPlaylist() {
            InitializeComponent();
            SimulateNotifications();

            for(int i = 0; i < 20; i++) {
                var card = new PlaylistCover();
                card.Margin = new Thickness(0, 0, 0, 15);
                PlaylistItemsControl.Items.Add(card);
            }
        }
        private void AccountButton_Click(object sender, RoutedEventArgs e) {
            var button = sender as Button;
            if(button?.ContextMenu != null) {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.IsOpen = true;
            }
        }

        private void Cuenta_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("Ir a la página de cuenta.");
        }

        private void CerrarSesion_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("Cerrar sesión...");
        }

        private void BtnGoToHome_Click_(object sender, RoutedEventArgs e) {
            if(this.NavigationService != null) {
                NavigationService.Navigate(new HomeListeners());
            }
        }
        private void SimulateNotifications() {
            for(int i = 1; i <= 10; i++) {
                notifications.Add(new Notification {
                    Title = $"Notificación {i}",

                });
            }

            LoadNotifications();
        }

        private void LoadNotifications() {
            NotificationList.Children.Clear();

            foreach(var notification in notifications) {
                var btn = new Button {
                    Width = 380,
                    Height = 70,
                    Margin = new Thickness(0, 5, 0, 5),
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Content = notification.Title,
                    Background = (Brush)new BrushConverter().ConvertFromString("#202123"),
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1),
                    Tag = notification // Guardamos la referencia
                };

                btn.Click += (s, e) => {
                    var noti = (Notification)((Button)s).Tag;
                    MessageBox.Show($"Navegar a: {noti.Title}");
                    LoadNotifications(); // Refrescar UI
                };

                NotificationList.Children.Add(btn);
            }
        }

        private void NotificationButton_Click(object sender, RoutedEventArgs e) {
            NotificationPopup.IsOpen = true;


            LoadNotifications();
        }


        public class Notification {
            public string Title { get; set; }
        }


        private void BtnSeeAllAlbums_Click(object sender, RoutedEventArgs e) {

        }

        private void BtnSeeAllArtists_Click(object sender, RoutedEventArgs e) {

        }

        private void BtnGoToCreatePlaylist_Click(object sender, RoutedEventArgs e) {

        }
    }
}

