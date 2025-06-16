using Daztl;
using DAZTLClient.Services;
using DAZTLClient.Windows.UserControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DAZTLClient.Windows
{
    public partial class GUI_ListenersArtists : Page
    {
        private List<Notification> notifications = new List<Notification>();
        private readonly ContentService contentService = new ContentService();
        private bool _isUserDraggingSlider = false;

        public GUI_ListenersArtists()
        {
            InitializeComponent();
            SimulateNotifications();
            _ = LoadArtistsAsync();

            // Configuración del reproductor de música
            SetupMusicPlayer();
        }

        // Métodos originales (sin cambios)
        private async Task LoadArtistsAsync()
        {
            try
            {
                ArtistItemsControl.Items.Clear();

                var response = await contentService.ListArtistsAsync();

                foreach (var artist in response.Artists)
                {
                    var card = new ArtistCover
                    {
                        Margin = new Thickness(0, 0, 0, 15),
                        ArtistName = artist.Name,
                        AlbumCover = artist.ProfilePicture,
                        Tag = artist
                    };

                    card.MouseLeftButtonDown += (s, e) =>
                    {
                        var artistCover = s as ArtistCover;
                        var selectedArtist = artistCover?.Tag as ArtistResponse;
                        if (selectedArtist != null)
                        {
                            PlayArtistSongs(selectedArtist.Id.ToString());
                        }
                    };

                    ArtistItemsControl.Items.Add(card);
                }

                if (response.Artists.Count == 0)
                {
                    MessageBox.Show("No se encontraron artistas.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar artistas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetupMusicPlayer()
        {
            PlayPauseToggle.IsChecked = MusicPlayerService.Instance.IsPlaying();
            MusicPlayerService.Instance.PlaybackPositionChanged += progress =>
            {
                if (!_isUserDraggingSlider)
                {
                    Dispatcher.Invoke(() => PlaybackSlider.Value = progress);
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

            // Configurar eventos del slider
            PlaybackSlider.PreviewMouseDown += PlaybackSlider_PreviewMouseDown;
            PlaybackSlider.PreviewMouseUp += PlaybackSlider_PreviewMouseUp;
            PlaybackSlider.ValueChanged += PlaybackSlider_ValueChanged;
        }

        private async void PlayArtistSongs(string artistId)
        {
            /*try
            {
                var response = await contentService.GetSongsByArtistAsync(artistId);
                if (response.Songs.Count > 0)
                {
                    var baseUrl = "http://localhost:8000";
                    var songUrls = response.Songs
                        .Select(s => baseUrl + s.AudioUrl)
                        .ToList();

                    MusicPlayerService.Instance.SetPlaylist(songUrls);
                    MusicPlayerService.Instance.PlayAt(0);

                    // Actualizar UI con la información del artista actual
                    var artistInfo = response.Artists.FirstOrDefault(a => a.Id == artistId);
                    if (artistInfo != null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            NowPlayingTitle.Text = artistInfo.Name;
                            NowPlayingArtist.Text = $"{response.Songs.Count} canciones";
                            NowPlayingImage.Source = new BitmapImage(new Uri(baseUrl + artistInfo.ProfilePicture));
                        });
                    }
                }
                else
                {
                    MessageBox.Show("Este artista no tiene canciones disponibles.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar canciones del artista: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }*/
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

        // Métodos originales (sin cambios)
        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.ContextMenu != null)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.IsOpen = true;
            }
        }

        private void Cuenta_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ir a la página de cuenta.");
        }

        private void CerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null)
            {
                NavigationService.Navigate(new GUI_LogIn());
                SessionManager.Instance.EndSession();
                MusicPlayerService.Instance.Stop();
            }
        }

        private void BtnGoToHome_Click_(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null)
            {
                NavigationService.Navigate(new HomeListeners());
            }
        }

        private void SimulateNotifications()
        {
            for (int i = 1; i <= 10; i++)
            {
                notifications.Add(new Notification
                {
                    Title = $"Notificación {i}",
                });
            }

            LoadNotifications();
        }

        private void LoadNotifications()
        {
            NotificationList.Children.Clear();

            foreach (var notification in notifications)
            {
                var btn = new Button
                {
                    Width = 380,
                    Height = 70,
                    Margin = new Thickness(0, 5, 0, 5),
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Content = notification.Title,
                    Background = (Brush)new BrushConverter().ConvertFromString("#202123"),
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1),
                    Tag = notification
                };

                btn.Click += (s, e) =>
                {
                    var noti = (Notification)((Button)s).Tag;
                    MessageBox.Show($"Navegar a: {noti.Title}");
                    LoadNotifications();
                };

                NotificationList.Children.Add(btn);
            }
        }

        private void NotificationButton_Click(object sender, RoutedEventArgs e)
        {
            NotificationPopup.IsOpen = true;
            LoadNotifications();
        }

        private void BtnSeeAllPlaylists_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null)
            {
                NavigationService.Navigate(new GUI_ListenersPlaylist());
            }
        }

        private void BtnSeeAllAlbums_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null)
            {
                NavigationService.Navigate(new GUI_ListenersAlbums());
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

            if (owner != null)
            {
                owner.Effect = null;
            }
        }

        public class Notification
        {
            public string Title { get; set; }
        }
    }
}