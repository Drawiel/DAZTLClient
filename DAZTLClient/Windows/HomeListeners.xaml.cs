﻿using DAZTLClient.Windows.UserControllers;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using static DAZTLClient.Windows.HomeListeners;
using System.Windows.Media;
using Daztl;
using System.Collections.ObjectModel;
using Grpc.Net.Client;
using DAZTLClient.Services;
using System.Windows.Input;
using System.Windows.Media.Imaging;

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

        public ObservableCollection<SongResponse> FilteredSongs { get; set; } = new();
        public ObservableCollection<AlbumResponse> FilteredAlbums { get; set; } = new();
        public ObservableCollection<ArtistResponse> FilteredArtists { get; set; } = new();
        public ObservableCollection<PlaylistResponse> FilteredPlaylists { get; set; } = new();
        private bool _isUserDraggingSlider = false;

        private readonly ContentService _contentService = new ContentService();

        public HomeListeners() {
            InitializeComponent();
            MusicPlayerService.Instance.PlaybackPositionChanged += progress =>
            {
                if (!_isUserDraggingSlider)
                {
                    PlaybackSlider.Value = progress;
                }
            };

            MusicPlayerService.Instance.PlaybackStateChanged += (isPlaying) =>
            {
                Dispatcher.Invoke(() => PlayPauseToggle.IsChecked = isPlaying);
            };

            PlayPauseToggle.Checked += (s, e) => MusicPlayerService.Instance.Resume();
            PlayPauseToggle.Unchecked += (s, e) => MusicPlayerService.Instance.Pause();

            PrevButton.Click += (s, e) => MusicPlayerService.Instance.PlayPrevious();
            NextButton.Click += (s, e) => MusicPlayerService.Instance.PlayNext();

            RepeatToggle.Checked += (s, e) => MusicPlayerService.Instance.IsRepeating = true;
            RepeatToggle.Unchecked += (s, e) => MusicPlayerService.Instance.IsRepeating = false;

            ShuffleToggle.Checked += (s, e) => MusicPlayerService.Instance.IsShuffling = true;
            ShuffleToggle.Unchecked += (s, e) => MusicPlayerService.Instance.IsShuffling = false;
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

            LoadAllSongs();
            LoadPlaylistPage();
            LoadAlbumsPage();
            LoadArtistsPage();
        }

        private async void LoadAllSongs()
        {
            try
            {
                var response = await _contentService.ListSongsAsync();

                var songControls = new[]
                {
            RecentSong1, RecentSon2, RecentSon3,
            RecentSon4, RecentSon5, RecentSon6,
            RecentSon7, RecentSon8, RecentSon9
        };

                for (int i = 0; i < songControls.Length; i++)
                {
                    if (i < response.Songs.Count)
                    {
                        var song = response.Songs[i];
                        songControls[i].SetSongData(song.Title, song.Artist, song.CoverUrl, song.AudioUrl);
                        songControls[i].Visibility = Visibility.Visible;
                    }
                    else
                    {
                        songControls[i].Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando canciones: {ex.Message}");
            }
        }
        private async void LoadPlaylistPage()
        {
            try
            {
                var reply = await _contentService.ListPlaylistsAsync();

                PlaylistGrid.Children.Clear();

                foreach (var playlist in reply.Playlists)
                {
                    var vm = new PlaylistViewModel
                    {
                        Id = playlist.Id,
                        Name = playlist.Name,
                        CoverUrl = playlist.CoverUrl
                    };

                    var cover = new PlaylistCover
                    {
                        DataContext = vm
                    };

                    PlaylistGrid.Children.Add(cover);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando playlists: {ex.Message}");
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

        private void BtnGoToProfileListener_Click(object sender, RoutedEventArgs e) {
            if(this.NavigationService != null) {
                NavigationService.Navigate(new GUI_ListenersProfile());
            }
        }

        private void Cuenta_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("Ir a la página de cuenta.");
        }

        private void CerrarSesion_Click(object sender, RoutedEventArgs e) {
            MusicPlayerService.Instance.Stop();
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

        private async void txtBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = txtBoxSearch.Text.Trim();

            if (string.IsNullOrWhiteSpace(query))
            {
                SearchPopup.IsOpen = false;
                return;
            }

            try
            {
                var response = await _contentService.GlobalSearchAsync(query);

                SearchResultsPanel.Children.Clear();

                if (response.Songs.Count == 0 &&
                    response.Albums.Count == 0 &&
                    response.Artists.Count == 0 &&
                    response.Playlists.Count == 0)
                {
                    SearchResultsPanel.Children.Add(new TextBlock
                    {
                        Text = "No se encontraron resultados.",
                        Foreground = Brushes.White,
                        Margin = new Thickness(5)
                    });
                }

                if (response.Songs.Count > 0)
                {
                    SearchResultsPanel.Children.Add(new TextBlock
                    {
                        Text = "Canciones",
                        FontSize = 28,
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.White,
                        Margin = new Thickness(5, 10, 5, 2)
                    });

                    foreach (var song in response.Songs)
                    {
                        var songPanel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Margin = new Thickness(10, 2, 5, 2),
                            Cursor = Cursors.Hand
                        };
                        var baseUrl = "http://localhost:8000";
                        var imageUrl = new Uri(baseUrl + song.CoverUrl);
                        var image = new Image
                        {
                            Source = new BitmapImage(imageUrl),
                            Width = 50,
                            Height = 50,
                            Margin = new Thickness(0, 0, 10, 0)
                        };

                        var text = new TextBlock
                        {
                            Text = $"{song.Title} - {song.Artist}",
                            FontSize = 24,
                            Foreground = Brushes.LightGray,
                            VerticalAlignment = VerticalAlignment.Center
                        };

                        songPanel.Children.Add(image);
                        songPanel.Children.Add(text);

                        songPanel.MouseLeftButtonDown += (s, args) =>
                        {
                            var audioUrl = baseUrl + song.AudioUrl;
                            if (string.IsNullOrEmpty(song.AudioUrl))
                            {
                                audioUrl = baseUrl + song.AudioUrl.Replace(".png", ".mp3");
                            }

                            MusicPlayerService.Instance.Play(audioUrl);

                            SearchPopup.IsOpen = false;
                        };

                        SearchResultsPanel.Children.Add(songPanel);
                    }


                }

                if (response.Albums.Count > 0)
                {
                    SearchResultsPanel.Children.Add(new TextBlock
                    {
                        Text = "Álbumes",
                        FontSize = 28,
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.White,
                        Margin = new Thickness(5, 10, 5, 2)
                    });

                    foreach (var album in response.Albums)
                    {
                        var albumItem = new TextBlock
                        {
                            Text = album.Title,
                            FontSize = 24,
                            Foreground = Brushes.LightGray,
                            Margin = new Thickness(10, 2, 5, 2),
                            Cursor = Cursors.Hand
                        };
                        albumItem.MouseLeftButtonDown += (s, args) =>
                        {
                            MessageBox.Show($"Seleccionaste el álbum: {album.Title}");
                            SearchPopup.IsOpen = false;
                        };
                        SearchResultsPanel.Children.Add(albumItem);
                    }
                }

                if (response.Artists.Count > 0)
                {
                    SearchResultsPanel.Children.Add(new TextBlock
                    {
                        Text = "Artistas",
                        FontSize = 28,
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.White,
                        Margin = new Thickness(5, 10, 5, 2)
                    });

                    foreach (var artist in response.Artists)
                    {
                        var artistItem = new TextBlock
                        {
                            Text = artist.Name,
                            FontSize = 24,
                            Foreground = Brushes.LightGray,
                            Margin = new Thickness(10, 2, 5, 2),
                            Cursor = Cursors.Hand
                        };
                        artistItem.MouseLeftButtonDown += (s, args) =>
                        {
                            MessageBox.Show($"Seleccionaste el artista: {artist.Name}");
                            SearchPopup.IsOpen = false;
                        };
                        SearchResultsPanel.Children.Add(artistItem);
                    }
                }

                if (response.Playlists.Count > 0)
                {
                    SearchResultsPanel.Children.Add(new TextBlock
                    {
                        Text = "Playlists",
                        FontSize = 28,
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.White,
                        Margin = new Thickness(5, 10, 5, 2)
                    });

                    foreach (var playlist in response.Playlists)
                    {
                        var playlistItem = new TextBlock
                        {
                            Text = playlist.Name,
                            FontSize = 24,
                            Foreground = Brushes.LightGray,
                            Margin = new Thickness(10, 2, 5, 2),
                            Cursor = Cursors.Hand
                        };
                        playlistItem.MouseLeftButtonDown += (s, args) =>
                        {
                            MessageBox.Show($"Seleccionaste la playlist: {playlist.Name}");
                            SearchPopup.IsOpen = false;
                        };
                        SearchResultsPanel.Children.Add(playlistItem);
                    }
                }

                SearchPopup.IsOpen = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                SearchPopup.IsOpen = false;
            }
        }
        private void PlaybackSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isUserDraggingSlider = true;
        }

        private void PlaybackSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isUserDraggingSlider = false;
            SeekToPosition(PlaybackSlider.Value);
        }

        private void PlaybackSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isUserDraggingSlider)
            {
                // No actualizar mientras arrastra, solo cuando suelta
                return;
            }
        }

        private void SeekToPosition(double sliderValue)
        {
            if (MusicPlayerService.Instance.CurrentDuration.TotalMilliseconds > 0)
            {
                var position = TimeSpan.FromMilliseconds(
                    sliderValue / 100 * MusicPlayerService.Instance.CurrentDuration.TotalMilliseconds);
                MusicPlayerService.Instance.Seek(position);
            }
        }


    }
}
