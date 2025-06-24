using Daztl;
using DAZTLClient.Models;
using DAZTLClient.Services;
using DAZTLClient.Windows.UserControllers;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace DAZTLClient.Windows
{
    public partial class GUI_AlbumDetails : Page
    {
        private readonly Daztl.AlbumDetailResponse _album =new();
        private bool _isUserDraggingSlider = false;
        private List<NotificationUnmarshalling> notifications = new List<NotificationUnmarshalling>();
        private readonly ContentService _contentService = new ContentService();

        private string currentFilesURL = "http://localhost:8000/media/";

        public GUI_AlbumDetails(int albumId)
        {
            InitializeComponent();
            LoadAlbumData(albumId);
            SetupMusicPlayer();
            MusicPlayerService.Instance.SongInfoChanged += UpdateNowPlayingInfo;
            if (MusicPlayerService.Instance.IsPlaying())
            {
                UpdateNowPlayingInfo();
            }
        }

        private void UpdateNowPlayingInfo()
        {
            Dispatcher.Invoke(() =>
            {
                var player = MusicPlayerService.Instance;
                var currentSong = player.CurrentSong;

                if (currentSong != null)
                {
                    BitmapImage albumCover = null;
                    if (!string.IsNullOrEmpty(currentSong.AlbumCoverUrl))
                    {
                        albumCover = new BitmapImage(new Uri(currentSong.AlbumCoverUrl));
                    }
                    SongPlayingNow3.SongTitle.Text = currentSong.Title ?? "Desconocido";
                    SongPlayingNow3.ArtistName.Text = currentSong.Artist ?? "Artista desconocido";
                    SongPlayingNow3.AlbumCover.Source = albumCover;
                }
                else
                {
                    SongPlayingNow3.SongTitle.Text = "No hay canción en reproducción";
                    SongPlayingNow3.ArtistName.Text = string.Empty;
                    SongPlayingNow3.AlbumCover.Source = null;
                }
            });
        }

        private async void LoadAlbumData(int albumId)
        {
            try
            {
                var albumDetail = await _contentService.GetsAlbumDetailAsync(albumId);

                _album.Title = albumDetail.Title;
                _album.CoverUrl = currentFilesURL + albumDetail.CoverUrl;
                _album.Songs.Clear();
                _album.Songs.AddRange(albumDetail.Songs);

                txtBlockTitle.Text = _album.Title;
                if (!string.IsNullOrEmpty(_album.CoverUrl))
                {
                    imgAlbumCover.Source = new BitmapImage(new Uri(_album.CoverUrl));
                }

                var songs = new List<SongViewModel>();
                foreach (var song in _album.Songs)
                {
                    songs.Add(new SongViewModel
                    {
                        Id = song.Id,
                        Title = song.Title,
                        Artist = song.Artist,
                        Image = new BitmapImage(new Uri(_album.CoverUrl)),
                        AudioUrl = currentFilesURL + song.AudioUrl
                    });
                }

                Songs.ItemsSource = songs;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos de la playlist: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

            MusicPlayerService.PlaybackStateChanged += OnPlaybackStateChanged;

            MusicPlayerService.Instance.ShuffleStateChanged += (isShuffling) =>
            {
                Dispatcher.Invoke(() => ShuffleToggle.IsChecked = isShuffling);
            };

            PlayPauseToggle.Checked += (s, e) => MusicPlayerService.Instance.Resume();
            PlayPauseToggle.Unchecked += (s, e) => MusicPlayerService.Instance.Pause();

            PrevButton.Click += (s, e) => MusicPlayerService.Instance.PlayPrevious();
            NextButton.Click += (s, e) => MusicPlayerService.Instance.PlayNext();

            RepeatToggle.Checked += (s, e) => MusicPlayerService.Instance.IsRepeating = true;
            RepeatToggle.Unchecked += (s, e) => MusicPlayerService.Instance.IsRepeating = false;

            ShuffleToggle.Checked += (s, e) => MusicPlayerService.Instance.IsShuffling = true;
            ShuffleToggle.Unchecked += (s, e) => MusicPlayerService.Instance.IsShuffling = false;

            PlaybackSlider.PreviewMouseDown += PlaybackSlider_PreviewMouseDown;
            PlaybackSlider.PreviewMouseUp += PlaybackSlider_PreviewMouseUp;
            PlaybackSlider.ValueChanged += PlaybackSlider_ValueChanged;
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

                        var imageUrl = new Uri(currentFilesURL + song.CoverUrl);
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
                            var audioUrl = currentFilesURL + song.AudioUrl;
                            if (string.IsNullOrEmpty(song.AudioUrl))
                            {
                                audioUrl = currentFilesURL + song.AudioUrl.Replace(".png", ".mp3");
                            }

                            MusicPlayerService.Instance.Play(
                                new SongInfo { Title = song.Title, Artist = song.Artist, AudioUrl = audioUrl, AlbumCoverUrl = currentFilesURL + song.CoverUrl }
                            );
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

                        var imageUrl = new Uri(currentFilesURL + album.CoverUrl);
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

                        var imageUrl = new Uri(currentFilesURL + artist.ProfilePicture.Replace("http://localhost/media/", ""));
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

                        var imageUrl = new Uri(currentFilesURL + playlist.CoverUrl);
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

        private void OnPlaybackStateChanged(bool isPlaying)
        {
            Dispatcher.Invoke(() => PlayPauseToggle.IsChecked = isPlaying);
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
        private void BtnGoToHome_Click_(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null)
            {
                NavigationService.Navigate(new HomeListeners());
            }
        }

        public class SongViewModel
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Artist { get; set; }
            public ImageSource Image { get; set; }
            public string AudioUrl { get; set; }
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.ContextMenu != null)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.IsOpen = true;
            }
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

        private void Cuenta_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new GUI_ListenersProfile());
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

        private void Song_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var song = (sender as FrameworkElement)?.DataContext as SongViewModel;
            if (song == null) return;

            try
            {
                var playlist = _album.Songs
                    .Select(s => new SongInfo
                    {
                        AudioUrl = currentFilesURL + s.AudioUrl,
                        Title = s.Title,
                        Artist = s.Artist,
                        AlbumCoverUrl = _album.CoverUrl
                    })
                    .ToList();

                MusicPlayerService.Instance.SetPlaylist(playlist);

                var selectedIndex = _album.Songs.ToList().FindIndex(s =>
                     s.AudioUrl == song.AudioUrl.Replace(currentFilesURL, ""));
                MusicPlayerService.Instance.PlayAt(selectedIndex);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al reproducir la canción: {ex.Message}",
                              "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            if (parentObject is T parent) return parent;
            return FindParent<T>(parentObject);
        }

    }
}