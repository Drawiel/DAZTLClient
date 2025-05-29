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
    /// Lógica de interacción para SingUp.xaml
    /// </summary>
    public partial class SingUp : UserControl {
        public event EventHandler LogInRequested;

        public SingUp() {
            InitializeComponent();

        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e) {
            LogInRequested?.Invoke(this, EventArgs.Empty);
        }
        private void btnHomeListener_Click(object sender, RoutedEventArgs e) {
            var homePage = new HomeListeners();

            NavigationService navigationService = NavigationService.GetNavigationService(this);

            if(navigationService != null) {
                navigationService.Navigate(homePage);
            } else {
                MessageBox.Show("No se encontró el servicio de navegación.");
            }
        }

    }

}