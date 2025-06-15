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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DAZTLClient.Windows {
    /// <summary>
    /// Lógica de interacción para GUI_LogIn.xaml
    /// </summary>
    public partial class GUI_LogIn : Page {
        public GUI_LogIn() {
            InitializeComponent();

            LoginPage.SignUpRequested += (s, e) => ShowSignUp();
            SignupPage.LogInRequested += (s, e) => ShowLogIn();
            LoginPage.LoginListenerSuccessful += (s, e) => NavigateToHomeListener();
            LoginPage.LoginArtistSuccessful += (s, e) => NavigateToHomeArtist();
        }

        private void ShowSignUp() {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));

            fadeOut.Completed += (s, e) => {
                LogIn.Visibility = Visibility.Collapsed;
                SignUp.Visibility = Visibility.Visible;
                SignUp.BeginAnimation(OpacityProperty, fadeIn);
            };

            LogIn.BeginAnimation(OpacityProperty, fadeOut);
        }

        private void ShowLogIn() {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));

            fadeOut.Completed += (s, e) => {
                SignUp.Visibility = Visibility.Collapsed;
                LogIn.Visibility = Visibility.Visible;
                LogIn.BeginAnimation(OpacityProperty, fadeIn);
            };

            SignUp.BeginAnimation(OpacityProperty, fadeOut);
        }

        private void NavigateToHomeListener()
        {
            this.NavigationService.Navigate(new HomeListeners());
        }

        private void NavigateToHomeArtist()
        {
            this.NavigationService.Navigate(new GUI_HomeArtist());
        }
    }

}