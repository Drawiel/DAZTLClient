using DAZTLClient.Windows.UserControllers;
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


namespace DAZTLClient.Windows
{
    /// <summary>
    /// Lógica de interacción para GUI_ListenersPlaylist.xaml
    /// </summary>
    public partial class GUI_ListenersPlaylist : Page {
        public GUI_ListenersPlaylist() {
            InitializeComponent();

            for(int i = 0; i < 20; i++) {
                var card = new PlaylistCover();
                card.Margin = new Thickness(0, 0, 0, 15);
                PlaylistItemsControl.Items.Add(card);
            }
        }
        private void AccountButton_Click(object sender, RoutedEventArgs e) {
            var button = sender as Button;
            if(button?.ContextMenu != null) {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.IsOpen = true;
            }
        }

        private void Cuenta_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("Ir a la página de cuenta.");
        }

        private void CerrarSesion_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("Cerrar sesión...");
        }

        private void BtnGoToHome_Click_(object sender, RoutedEventArgs e) {
            if(this.NavigationService != null) {
                NavigationService.Navigate(new HomeListeners());
            }
        }
    }
}
