using DAZTLClient.Windows.UserControllers;
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
using System.Windows.Media.Effects;
using static DAZTLClient.Windows.UserControllers.ArtistCover;
using System.Xml.Serialization;
using System.Text.Json;
using DAZTLClient.Models;

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
        private List<PlaylistResponse> _allPlaylists = new List<PlaylistResponse>();
        private List<AlbumViewModel> allAlbums = new List<AlbumViewModel>();
        private List<ArtistViewModel> allArtists = new List<ArtistViewModel>();

        private List<NotificationUnmarshalling> notifications = new List<NotificationUnmarshalling>();
        private bool hasUnreadNotifications = true;

        public ObservableCollection<SongResponse> FilteredSongs { get; set; } = new();
        public ObservableCollection<AlbumResponse> FilteredAlbums { get; set; } = new();
        public ObservableCollection<ArtistResponse> FilteredArtists { get; set; } = new();
        public ObservableCollection<PlaylistResponse> FilteredPlaylists { get; set; } = new();
        private bool _isUserDraggingSlider = false;

        private readonly ContentService _contentService = new ContentService();

        public HomeListeners() {
            InitializeComponent();
            PlayPauseToggle.IsChecked = MusicPlayerService.Instance.IsPlaying();
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
            NextButtonPlaylists.Click += (s, e) => MusicPlayerService.Instance.PlayNext();

            RepeatToggle.Checked += (s, e) => MusicPlayerService.Instance.IsRepeating = true;
            RepeatToggle.Unchecked += (s, e) => MusicPlayerService.Instance.IsRepeating = false;

            ShuffleToggle.Checked += (s, e) => MusicPlayerService.Instance.IsShuffling = true;
            ShuffleToggle.Unchecked += (s, e) => MusicPlayerService.Instance.IsShuffling = false;
            allCovers = new List<PlaylistCover>();

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
            _ = LoadNotificationsFromDBAsync(); 
            _ = StartWebSocketListener();
        }

        private async Task LoadNotificationsFromDBAsync()
        {
            var notificationsDB = await _contentService.ListNotificationsAsync();

            foreach (var grpcNotification in notificationsDB.Notifications)
            {
                notifications.Add(new NotificationUnmarshalling
                {
                    id = int.Parse(grpcNotification.Id),
                    message = grpcNotification.Message,
                    created_at = DateTime.Parse(grpcNotification.CreatedAt)
                });
            }

        }

        private async Task StartWebSocketListener()
        {
            var client = new WebSocketClientNotification(this);
            await client.StartAsync();
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
                _allPlaylists = reply.Playlists.ToList();
                currentPagePlaylist = 0;
                RenderPlaylistPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando playlists: {ex.Message}");
            }
        }

        private void RenderPlaylistPage()
        {
            PlaylistGrid.Children.Clear();

            int start = currentPagePlaylist * itemsPerPage;
            int end = Math.Min(start + itemsPerPage, _allPlaylists.Count);

            for (int i = start; i < end; i++)
            {
                var playlistResponse = _allPlaylists[i];
                var cover = new PlaylistCover
                {
                    DataContext = playlistResponse
                };
                cover.PlaylistClicked += PlaylistCover_PlaylistClicked;
                PlaylistGrid.Children.Add(cover);
            }
            NextButtonPlaylists.IsEnabled = (currentPagePlaylist + 1) * itemsPerPage < _allPlaylists.Count;
        }

        private void PlaylistCover_PlaylistClicked(object sender, PlaylistResponse playlistResponse)
        {
            if (this.NavigationService != null)
            {
                NavigationService.Navigate(new GUI_PlaylistDetails(playlistResponse));
            }
        }

        private void NextPagePlaylists_Click(object sender, RoutedEventArgs e)
        {
            if ((currentPagePlaylist + 1) * itemsPerPage < _allPlaylists.Count)
            {
                currentPagePlaylist++;
                RenderPlaylistPage();
            }
        }
        private async void LoadAlbumsPage() {
            try
            {
                var reply = await _contentService.ListAlbumsAsync();

                allAlbums = reply.Albums.Select(album => new AlbumViewModel
                {
                    Id = album.Id,
                    Title = album.Title,
                    ArtistName = album.ArtistName,
                    CoverUrl = album.CoverUrl
                }).ToList();

                currentPageAlbums = 0;
                RenderAlbumsPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando álbumes: {ex.Message}");
            }
        }

        private void RenderAlbumsPage()
        {
            AlbumsGrid.Children.Clear();

            int start = currentPageAlbums * itemsPerPage;
            int end = start + itemsPerPage;

            for (int i = start; i < end && i < allAlbums.Count; i++)
            {
                var album = allAlbums[i];

                var albumCover = new AlbumCover
                {
                    DataContext = album
                };
                albumCover.AlbumClicked += (sender, Empty) =>
                {
                    if (this.NavigationService != null)
                    {
                        NavigationService.Navigate(new GUI_AlbumDetails(album.Id));
                    }
                };
                AlbumsGrid.Children.Add(albumCover);
            }

            NextButtonAlbums.IsEnabled = (currentPageAlbums + 1) * itemsPerPage < allAlbums.Count;
        }

        private void NextPageAlbums_Click(object sender, RoutedEventArgs e)
        {
            if ((currentPageAlbums + 1) * itemsPerPage < allAlbums.Count)
            {
                currentPageAlbums++;
                RenderAlbumsPage();
            }
        }

        private async void LoadArtistsPage()
        {
            try
            {
                var reply = await _contentService.ListArtistsAsync();

                allArtists = reply.Artists.Select(artist => new ArtistViewModel
                {
                    Id = artist.Id,
                    Name = artist.Name,
                    ProfilePicture = artist.ProfilePicture
                }).ToList();

                currentPageArtists = 0;
                RenderArtistsPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando artistas: {ex.Message}");
            }
        }

        private void RenderArtistsPage()
        {
            ArtistGrid.Children.Clear();
            int start = currentPageArtists * itemsPerPage;
            int end = start + itemsPerPage;

            for (int i = start; i < end && i < allArtists.Count; i++)
            {
                var artist = allArtists[i];
                var artistControl = new ArtistCover
                {
                    ArtistId = artist.Id,
                    ArtistName = artist.Name,
                    AlbumCover = artist.ProfilePicture
                };
                ArtistGrid.Children.Add(artistControl);
            }
            BtnNextPageArtists.IsEnabled = (currentPageAlbums + 1) * itemsPerPage < allAlbums.Count;
        }
        private void NextPageArtists_Click(object sender, RoutedEventArgs e)
        {
            if ((currentPageArtists + 1) * itemsPerPage < allArtists.Count)
            {
                currentPageArtists++;
                RenderArtistsPage();
            }
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

        private void Logout_Click(object sender, RoutedEventArgs e) {
            if (this.NavigationService != null)
            {
                NavigationService.Navigate(new GUI_LogIn());
                SessionManager.Instance.EndSession();
                MusicPlayerService.Instance.Stop();
            }
        }

        private void BtnSeeAllPlaylist_Click(object sender, RoutedEventArgs e) {
            if(this.NavigationService != null) {
                NavigationService.Navigate(new GUI_ListenersPlaylist());
            }
        }
        
        public void AddNotification(string titleNotification)
        {
            var notification = JsonSerializer.Deserialize<NotificationUnmarshalling>(titleNotification);

            if (notification?.type == "notification")
            {
                notifications.Add(notification);
            }
        }

        private void LoadNotifications()
        {
            NotificationList.Children.Clear();

            foreach (var notification in notifications.ToList()) 
            {
                var btn = new Button
                {
                    Width = 380,
                    Height = 70,
                    Margin = new Thickness(0, 5, 0, 5),
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Content = new TextBlock
                    {
                        Text = notification.message,
                        TextWrapping = TextWrapping.Wrap,
                        TextTrimming = TextTrimming.CharacterEllipsis,
                        MaxWidth = 360 
                    },
                    Background = (Brush)new BrushConverter().ConvertFromString("#202123"),
                    BorderBrush = Brushes.Gray,
                    Foreground = Brushes.White,
                    BorderThickness = new Thickness(1),
                    Tag = notification
                };

                btn.Click += (s, e) =>
                {
                    var noti = (NotificationUnmarshalling)((Button)s).Tag;
                    DateTime notificationTime = noti.created_at;
                    TimeSpan elapsed = DateTime.Now - notificationTime;
                    if (elapsed.TotalMinutes > 50)
                    {
                        MessageBox.Show("El chat ha expirado (más de 50 minutos).");
                    }
                    else
                    {
                        _ = _contentService.DeleteNotificationAsync(noti.id);
                        var chatWindow = new ChatWindow("");
                        chatWindow.Owner = Application.Current.MainWindow;
                        chatWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        chatWindow.Show();
                    }
                    

                    notifications.Remove(noti);
                    LoadNotifications();
                };

                NotificationList.Children.Add(btn);
            }
        }


        private void NotificationButton_Click(object sender, RoutedEventArgs e) {
            NotificationPopup.IsOpen = true;


            LoadNotifications();
        }



        private void BtnSeeAllAlbums_Click(object sender, RoutedEventArgs e) {
            if (this.NavigationService != null)
            {
                NavigationService.Navigate(new GUI_ListenersAlbums());
            }
        }

        private void BtnSeeAllArtists_Click(object sender, RoutedEventArgs e) {
            if (this.NavigationService != null)
            {
                NavigationService.Navigate(new GUI_ListenersArtists());
            }
        }
        private void BtnGoToCreatePlaylist_Click(object sender, RoutedEventArgs e)
        {
            var owner = Window.GetWindow(this);

            if (owner != null)
            {
                owner.Effect = new BlurEffect { Radius = 5 };
            }

            CreatePlaylistWindow createPlaylistWindow = new CreatePlaylistWindow();
            createPlaylistWindow.Owner = owner;
            createPlaylistWindow.ShowDialog();
            LoadAlbumsPage();
            if (owner != null)
            {
                owner.Effect = null;
            }
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

                var baseUrl = "http://localhost:8000";

                // Canciones (ya implementado)
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

                // Álbumes (modificado para mostrar imágenes)
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
                        var albumPanel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Margin = new Thickness(10, 2, 5, 2),
                            Cursor = Cursors.Hand
                        };

                        var imageUrl = new Uri(baseUrl + album.CoverUrl);
                        var image = new Image
                        {
                            Source = new BitmapImage(imageUrl),
                            Width = 50,
                            Height = 50,
                            Margin = new Thickness(0, 0, 10, 0)
                        };

                        var text = new TextBlock
                        {
                            Text = $"{album.Title} - {album.ArtistName}",
                            FontSize = 24,
                            Foreground = Brushes.LightGray,
                            VerticalAlignment = VerticalAlignment.Center
                        };

                        albumPanel.Children.Add(image);
                        albumPanel.Children.Add(text);

                        albumPanel.MouseLeftButtonDown += (s, args) =>
                        {
                            MessageBox.Show($"Seleccionaste el álbum: {album.Title}");
                            SearchPopup.IsOpen = false;
                        };

                        SearchResultsPanel.Children.Add(albumPanel);
                    }
                }

                // Artistas (modificado para mostrar imágenes)
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
                        var artistPanel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Margin = new Thickness(10, 2, 5, 2),
                            Cursor = Cursors.Hand
                        };

                        var imageUrl = new Uri(baseUrl + artist.ProfilePicture);
                        var image = new Image
                        {
                            Source = new BitmapImage(imageUrl),
                            Width = 50,
                            Height = 50,
                            Margin = new Thickness(0, 0, 10, 0)
                        };

                        var text = new TextBlock
                        {
                            Text = artist.Name,
                            FontSize = 24,
                            Foreground = Brushes.LightGray,
                            VerticalAlignment = VerticalAlignment.Center
                        };

                        artistPanel.Children.Add(image);
                        artistPanel.Children.Add(text);

                        artistPanel.MouseLeftButtonDown += (s, args) =>
                        {
                            MessageBox.Show($"Seleccionaste el artista: {artist.Name}");
                            SearchPopup.IsOpen = false;
                        };

                        SearchResultsPanel.Children.Add(artistPanel);
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
                        var playlistPanel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Margin = new Thickness(10, 2, 5, 2),
                            Cursor = Cursors.Hand
                        };

                        var imageUrl = new Uri(baseUrl + playlist.CoverUrl);
                        var image = new Image
                        {
                            Source = new BitmapImage(imageUrl),
                            Width = 50,
                            Height = 50,
                            Margin = new Thickness(0, 0, 10, 0)
                        };

                        var text = new TextBlock
                        {
                            Text = playlist.Name,
                            FontSize = 24,
                            Foreground = Brushes.LightGray,
                            VerticalAlignment = VerticalAlignment.Center
                        };

                        playlistPanel.Children.Add(image);
                        playlistPanel.Children.Add(text);

                        playlistPanel.MouseLeftButtonDown += (s, args) =>
                        {
                            NavigationService.Navigate(new GUI_PlaylistDetails(playlist));
                        };

                        SearchResultsPanel.Children.Add(playlistPanel);
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
