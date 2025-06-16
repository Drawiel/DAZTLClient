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

namespace DAZTLClient.Windows.UserControllers
{
    /// <summary>
    /// Interaction logic for AlbumCover.xaml
    /// </summary>
    public partial class AlbumCover : UserControl
    {
        public AlbumCover()
        {
            InitializeComponent();
        }

        private void btnAlbum_Click(object sender, RoutedEventArgs e)
        {

        }
    }
    public class AlbumViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public string CoverUrl { get; set; }
    }
}
