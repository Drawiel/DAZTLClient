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

namespace DAZTLClient.Windows {
    /// <summary>
    /// Lógica de interacción para GUI_HomeArtist.xaml
    /// </summary>
    public partial class GUI_HomeArtist : Page {
        public GUI_HomeArtist() {
            InitializeComponent();
        }

        private void BtnGoToProfile_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null)
            {
                NavigationService.Navigate(new GUI_ArtistProfile());
            }
        }
    }
}
