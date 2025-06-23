using DAZTLClient.Services;
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

namespace DAZTLClient.Windows.UserControllers {
    /// <summary>
    /// Lógica de interacción para RecentSong.xaml
    /// </summary>
    public partial class RecentSong : UserControl {
        public RecentSong() {
            InitializeComponent();
        }

        public string SongTitle {
            get { return (string)GetValue(SongTitleProperty); }
            set { SetValue(SongTitleProperty, value); }
        }

        public static readonly DependencyProperty SongTitleProperty =
            DependencyProperty.Register("SongTitle", typeof(string), typeof(RecentSong), new PropertyMetadata(""));

        public string ArtistName {
            get { return (string)GetValue(ArtistNameProperty); }
            set { SetValue(ArtistNameProperty, value); }
        }

        public static readonly DependencyProperty ArtistNameProperty =
            DependencyProperty.Register("ArtistName", typeof(string), typeof(RecentSong), new PropertyMetadata(""));

        public ImageSource AlbumCover {
            get { return (ImageSource)GetValue(AlbumCoverProperty); }
            set { SetValue(AlbumCoverProperty, value); }
        }

        public static readonly DependencyProperty AlbumCoverProperty =
            DependencyProperty.Register("AlbumCover", typeof(ImageSource), typeof(RecentSong), new PropertyMetadata(null));

        public string AudioUrl
        {
            get { return (string)GetValue(AudioUrlProperty); }
            set { SetValue(AudioUrlProperty, value); }
        }

        public static readonly DependencyProperty AudioUrlProperty =
            DependencyProperty.Register("AudioUrl", typeof(string), typeof(RecentSong), new PropertyMetadata(""));


        public void SetSongData(string title, string artist, string coverUrl, string audioUrl = "")
        {
            SongTitle = title;
            ArtistName = artist;
            AudioUrl = audioUrl;

            if (!string.IsNullOrEmpty(coverUrl))
            {
                try
                {
                    var uri = new Uri(coverUrl, UriKind.Absolute);
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = uri;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    AlbumCover = image;
                }
                catch
                {
                    AlbumCover = null;
                }
            }
            else
            {
                AlbumCover = null;
            }

            this.Visibility = (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(artist)) ? Visibility.Collapsed : Visibility.Visible;
        }
        private void btnRecentSong_Click(object sender, RoutedEventArgs e)
        {
            string baseUrl = "http://localhost:8000";
            var audioUrl = AudioUrl;

            if (!string.IsNullOrEmpty(audioUrl))
            {
                if (!audioUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    audioUrl = baseUrl + audioUrl;
                }
            }
            else
            {
                audioUrl = AlbumCover?.ToString()?.Replace(".png", ".mp3");
                if (!string.IsNullOrEmpty(audioUrl) && !audioUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    audioUrl = baseUrl + audioUrl;
                }
            }

            if (!string.IsNullOrEmpty(audioUrl))
            {
                MusicPlayerService.Instance.Play(audioUrl, SongTitle, ArtistName, AlbumCover?.ToString());
            }
            else
            {
                MessageBox.Show("No hay audio disponible para esta canción.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }

}
