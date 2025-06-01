using DAZTLClient.Windows.UserControllers;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using static DAZTLClient.Windows.HomeListeners;
using System.Windows.Media;

namespace DAZTLClient.Windows {
    /// <summary>
    /// Lógica de interacción para HomeListeners.xaml
    /// </summary>
    public partial class HomeListeners : Page {
        private int currentPagePlaylist = 0;
        private int currentPageAlbums = 0;
        private int currentPageArtists = 0;
        private int itemsPerPage = 6;

        private List<PlaylistCover> allCovers;

        private List<Notification> notifications = new List<Notification>();
        private bool hasUnreadNotifications = true;

        public HomeListeners() {
            InitializeComponent();
            allCovers = new List<PlaylistCover>();
            SimulateNotifications();

            for(int i = 0; i < 12; i++) {
                var cover = new PlaylistCover {
                    SongTitle = $"Canción {i + 1}",
                    ArtistName = $"Artista {i + 1}",
                    AlbumCover = "Images/album-placeholder.png"
                };

                allCovers.Add(cover);
            }

            LoadPlaylistPage();
            LoadAlbumsPage();
            LoadArtistsPage();
        }

        private void LoadPlaylistPage() {
            PlaylistGrid.Children.Clear();
            int start = currentPagePlaylist * itemsPerPage;
            int end = start + itemsPerPage;

            for(int i = start; i < end && i < allCovers.Count; i++) {
                var original = allCovers[i];
                var playlistCover = new PlaylistCover {
                    SongTitle = original.SongTitle,
                    ArtistName = original.ArtistName,
                    AlbumCover = original.AlbumCover
                };
                PlaylistGrid.Children.Add(playlistCover);
            }
        }

        private void LoadAlbumsPage() {
            AlbumsGrid.Children.Clear();
            int start = currentPageAlbums * itemsPerPage;
            int end = start + itemsPerPage;

            for(int i = start; i < end && i < allCovers.Count; i++) {
                var original = allCovers[i];
                var albumCover = new PlaylistCover {
                    SongTitle = original.SongTitle,
                    ArtistName = original.ArtistName,
                    AlbumCover = original.AlbumCover
                };
                AlbumsGrid.Children.Add(albumCover);
            }
        }

        private void LoadArtistsPage() {
            ArtistGrid.Children.Clear();
            int start = currentPageArtists * itemsPerPage;
            int end = start + itemsPerPage;

            for(int i = start; i < end && i < allCovers.Count; i++) {
                var original = allCovers[i];
                var artistCover = new PlaylistCover {
                    SongTitle = original.SongTitle,
                    ArtistName = original.ArtistName,
                    AlbumCover = original.AlbumCover
                };
                ArtistGrid.Children.Add(artistCover);
            }
        }

        private void NextPagePlaylists_Click(object sender, RoutedEventArgs e) {
            currentPagePlaylist++;
            if(currentPagePlaylist * itemsPerPage >= allCovers.Count)
                currentPagePlaylist = 0;

            LoadPlaylistPage();
        }

        private void NextPageAlbums_Click(object sender, RoutedEventArgs e) {
            currentPageAlbums++;
            if(currentPageAlbums * itemsPerPage >= allCovers.Count)
                currentPageAlbums = 0;

            LoadAlbumsPage();
        }

        private void NextPageArtists_Click(object sender, RoutedEventArgs e) {
            currentPageArtists++;
            if(currentPageArtists * itemsPerPage >= allCovers.Count)
                currentPageArtists = 0;

            LoadArtistsPage();
        }

        private void btnRecentSong_Click(object sender, RoutedEventArgs e) {
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

        private void BtnSeeAllPlaylist_Click(object sender, RoutedEventArgs e) {
            if(this.NavigationService != null) {
                NavigationService.Navigate(new GUI_ListenersPlaylist());
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
