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

        private void btnRecentSong_Click(object sender, RoutedEventArgs e) {

        }
    }

}
