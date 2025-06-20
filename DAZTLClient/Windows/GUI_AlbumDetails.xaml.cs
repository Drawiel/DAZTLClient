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

            PlaybackSlider.PreviewMouseDown += PlaybackSlider_PreviewMouseDown;
            PlaybackSlider.PreviewMouseUp += PlaybackSlider_PreviewMouseUp;
            PlaybackSlider.ValueChanged += PlaybackSlider_ValueChanged;
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
            if (e.OriginalSource is DependencyObject source)
            {
                var parent = FindParent<Button>(source);
                if (parent != null) return;
            }

            if (sender is Border border && border.DataContext is SongViewModel song)
            {
                try
                {
                    var audioUrl = song.AudioUrl;

                    var playlist = _album.Songs
                        .Select(s => s.AudioUrl)
                        .ToList();

                    MusicPlayerService.Instance.SetPlaylist(playlist);
                    MusicPlayerService.Instance.PlayAt(playlist.IndexOf(song.AudioUrl));

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al reproducir la canción: {ex.Message}",
                                  "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            if (parentObject is T parent) return parent;
            return FindParent<T>(parentObject);
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
                var response = await _contentService.SearchSongsAsync(query);

                SearchResultsPanel.Children.Clear();

                if (response.Songs.Count == 0)
                {
                    SearchResultsPanel.Children.Add(new TextBlock
                    {
                        Text = "No se encontraron canciones.",
                        Foreground = Brushes.White,
                        Margin = new Thickness(5)
                    });
                    SearchPopup.IsOpen = true;
                    return;
                }

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
                        Margin = new Thickness(10, 5, 5, 5),
                        Cursor = Cursors.Hand
                    };

                    var imageUrl = new Uri(song.CoverUrl);
                    var image = new Image
                    {
                        Source = new BitmapImage(imageUrl),
                        Width = 50,
                        Height = 50,
                        Margin = new Thickness(0, 0, 10, 0)
                    };

                    var textPanel = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        VerticalAlignment = VerticalAlignment.Center,
                        Width = 1000
                    };

                    var titleText = new TextBlock
                    {
                        Text = song.Title,
                        FontSize = 20,
                        Foreground = Brushes.White,
                        FontWeight = FontWeights.SemiBold,
                        TextTrimming = TextTrimming.CharacterEllipsis
                    };

                    var artistText = new TextBlock
                    {
                        Text = song.Artist,
                        FontSize = 16,
                        Foreground = Brushes.LightGray,
                        TextTrimming = TextTrimming.CharacterEllipsis
                    };

                    textPanel.Children.Add(titleText);
                    textPanel.Children.Add(artistText);
                    songPanel.Children.Add(image);
                    songPanel.Children.Add(textPanel);

                    songPanel.MouseLeftButtonDown += (s, args) =>
                    {
                        var audioUrl = song.AudioUrl;
                        if (string.IsNullOrEmpty(song.AudioUrl))
                        {
                            audioUrl = song.AudioUrl;
                        }

                        MusicPlayerService.Instance.Play(audioUrl);
                        SearchPopup.IsOpen = false;
                    };

                    SearchResultsPanel.Children.Add(songPanel);
                }

                SearchPopup.IsOpen = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                SearchPopup.IsOpen = false;
            }
        }
    }
}