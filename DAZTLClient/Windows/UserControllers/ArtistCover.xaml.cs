using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Daztl;
using DAZTLClient.Services;
using Grpc.Net.Client;

namespace DAZTLClient.Windows.UserControllers
{
    public partial class ArtistCover : UserControl
    {
        private readonly ContentService _contentService = new ContentService();
        public ArtistCover()
        {
            InitializeComponent();
            this.DataContext = this;
            Loaded += ArtistCover_Loaded;
        }

        private async void ArtistCover_Loaded(object sender, RoutedEventArgs e)
        {
            if (ArtistId > 0)
            {
                await CheckLikeStatus();
            }
        }

        private async Task CheckLikeStatus()
        {
            try
            {
                IsLiked = await _contentService.IsArtistLikedAsync(ArtistId);
                UpdateLikeImage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking like status: {ex.Message}");
            }
        }

        private void UpdateLikeImage()
        {
            try
            {
                var imagePath = IsLiked ?
                    "pack://application:,,,/Multimedia/icons/like_blue.png" :
                    "pack://application:,,,/Multimedia/icons/like_black.png";

                imgLike.Source = new BitmapImage(new Uri(imagePath));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}");
            }
        }

        private async void btnLike_Click(object sender, RoutedEventArgs e)
        {
            if (ArtistId <= 0) return;

            try
            {
                await _contentService.ToggleArtistLikeAsync(ArtistId);
                IsLiked = !IsLiked;
                UpdateLikeImage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating like: {ex.Message}");
            }
        }

        private void btnRecentSong_Click(object sender, RoutedEventArgs e)
        {
        }

        public static readonly DependencyProperty SongTitleProperty =
            DependencyProperty.Register("SongTitle", typeof(string), typeof(ArtistCover), new PropertyMetadata(""));

        public string SongTitle
        {
            get => (string)GetValue(SongTitleProperty);
            set => SetValue(SongTitleProperty, value);
        }

        public static readonly DependencyProperty ArtistNameProperty =
            DependencyProperty.Register("ArtistName", typeof(string), typeof(ArtistCover), new PropertyMetadata(""));

        public string ArtistName
        {
            get => (string)GetValue(ArtistNameProperty);
            set => SetValue(ArtistNameProperty, value);
        }

        public static readonly DependencyProperty AlbumCoverProperty =
            DependencyProperty.Register("AlbumCover", typeof(string), typeof(ArtistCover), new PropertyMetadata(""));

        public string AlbumCover
        {
            get => (string)GetValue(AlbumCoverProperty);
            set => SetValue(AlbumCoverProperty, value);
        }

        public static readonly DependencyProperty ArtistIdProperty =
            DependencyProperty.Register("ArtistId", typeof(int), typeof(ArtistCover), new PropertyMetadata(0));

        public int ArtistId
        {
            get => (int)GetValue(ArtistIdProperty);
            set => SetValue(ArtistIdProperty, value);
        }

        public static readonly DependencyProperty IsLikedProperty =
            DependencyProperty.Register("IsLiked", typeof(bool), typeof(ArtistCover), new PropertyMetadata(false));

        public bool IsLiked
        {
            get => (bool)GetValue(IsLikedProperty);
            set => SetValue(IsLikedProperty, value);
        }
        public class ArtistViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string ProfilePicture { get; set; }
            public bool IsLiked { get; set; } 
        }
    }
}