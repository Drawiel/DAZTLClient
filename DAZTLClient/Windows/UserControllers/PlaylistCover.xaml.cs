﻿using System;
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

namespace DAZTLClient.Windows.UserControllers
{
    /// <summary>
    /// Lógica de interacción para PlaylistCover.xaml
    /// </summary>
    public partial class PlaylistCover : UserControl
    {
        public PlaylistCover()
        {
            InitializeComponent();
        }
        private void btnRecentSong_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("Botón clickeado");
        }
        public static readonly DependencyProperty SongTitleProperty =
        DependencyProperty.Register("SongTitle", typeof(string), typeof(PlaylistCover), new PropertyMetadata(""));

        public string SongTitle {
            get => (string)GetValue(SongTitleProperty);
            set => SetValue(SongTitleProperty, value);
        }

        public static readonly DependencyProperty ArtistNameProperty =
            DependencyProperty.Register("ArtistName", typeof(string), typeof(PlaylistCover), new PropertyMetadata(""));

        public string ArtistName {
            get => (string)GetValue(ArtistNameProperty);
            set => SetValue(ArtistNameProperty, value);
        }

        public static readonly DependencyProperty AlbumCoverProperty =
            DependencyProperty.Register("AlbumCover", typeof(string), typeof(PlaylistCover), new PropertyMetadata(""));

        public string AlbumCover {
            get => (string)GetValue(AlbumCoverProperty);
            set => SetValue(AlbumCoverProperty, value);
        }

        private void btnPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PlaylistViewModel vm)
            {
                MessageBox.Show($"Clic en playlist: {vm.Name} (ID: {vm.Id})");
            }
        }
    }
        public class PlaylistViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string CoverUrl { get; set; }
        }
}
