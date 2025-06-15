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
    /// Lógica de interacción para GUI_ArtistProfile.xaml
    /// </summary>
    public partial class GUI_ArtistProfile : Page {
        public GUI_ArtistProfile() {
            InitializeComponent();

        }
        private void AccountButton_Click(object sender, RoutedEventArgs e) {
            var button = sender as Button;
            if(button?.ContextMenu != null) {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.IsOpen = true;
            }
        }
      
        private void CerrarSesion_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("Cerrar sesión...");
        }

        private void BtnGoToHome_Click_(object sender, RoutedEventArgs e) {
            if(this.NavigationService != null) {
                NavigationService.Navigate(new GUI_HomeArtist());
            }
        } private void BtnSeeAlbumsArtist_Click_(object sender, RoutedEventArgs e) {
        }
       
        private void BtnEditUsername_Click(object sender, RoutedEventArgs e) {
            txtUsername.IsReadOnly = false;
            txtUsername.Focus();
            txtImage.IsReadOnly = false;
            txtPassword.IsReadOnly = false;
            btnSave.Visibility = Visibility.Visible;
            btnCancel.Visibility = Visibility.Visible;
        }

        private void BtnGotoAddSong_Click(object sender, RoutedEventArgs e) {

        }

        private void BtnGotoAddAlbum_Click(object sender, RoutedEventArgs e) {

        }

        private void BtnSeeAlbumsArtist_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}




