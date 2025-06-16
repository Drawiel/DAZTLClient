using DAZTLClient.Windows.UserControllers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace DAZTLClient.Windows {
    /// <summary>
    /// Lógica de interacción para GUI_PlaylistDetails.xaml
    /// </summary>
    public partial class GUI_PlaylistDetails : Page {
        private List<Notification> notifications = new List<Notification>();
        public GUI_PlaylistDetails() {
            InitializeComponent();
            SimulateNotifications();

            // Creamos la instancia del ViewModel y se lo asignamos al DataContext
            var playlist = new PlaylistViewModel {
                PlaylistImage = "Images/playlist.png", // Asegúrate de que esta imagen exista en tu carpeta Images
                PlaylistTitle = "Tus me gusta",
                Username = "Isabella",
                Songs = new ObservableCollection<Song>
                    {
                    new Song { Title = "i want it", Artist = "Ally Bakst", Image = "Images/song1.png" },
                    new Song { Title = "ceilings", Artist = "Lizzy McAlpine", Image = "Images/song2.png" },
                    new Song { Title = "Leave Me Alone", Artist = "Reneé Rapp", Image = "Images/song3.png" }
                }
            };

            DataContext = playlist;
        }

        public class Song {
            public string Title { get; set; }
            public string Artist { get; set; }
            public string Image { get; set; } // Ruta de imagen
        }

        public class PlaylistViewModel {
            public string PlaylistImage { get; set; }
            public string PlaylistTitle { get; set; }
            public string Username { get; set; }
            public ObservableCollection<Song> Songs { get; set; }
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


        private void BtnSeeAllPlaylists_Click(object sender, RoutedEventArgs e) {

        }

        private void BtnSeeAllArtists_Click(object sender, RoutedEventArgs e) {

        }

        private void BtnGoToCreatePlaylist_Click(object sender, RoutedEventArgs e) {

        }
    }
}